using System.Globalization;
using Sprache;

namespace CsFeel;

public static class FeelParser
{
    // Entry point (using Ref to allow recursion)
    public static readonly Parser<FeelExpression> Expr = Parse.Ref(() => _fullExpr);

    // ____________ 1. expressions ___________//

    // literals: null, bool, number, string and identifiers
    static readonly Parser<FeelExpression> _null =
        from _nill in Parse.String("null").Token() select new FeelLiteral(null);
    static readonly Parser<FeelExpression> _bool =
       from boolStr in Parse.String("true").Or(Parse.String("false")).Token().Text()
       select new FeelLiteral(bool.Parse(boolStr));
    static readonly Parser<FeelExpression> _number =
        Parse.Regex(@"\d+\.\d+|\.\d+|\d+").Select(value => new FeelLiteral(
            decimal.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture)));
    static readonly Parser<FeelExpression> _string =
        from _lq in Parse.Char('"')
        from content in Parse.CharExcept('"').Many().Text()
        from _rq in Parse.Char('"')
        select new FeelLiteral(content);
    static readonly Parser<FeelExpression> _identifier =
        Parse.Identifier(Parse.Letter, Parse.LetterOrDigit).Select(name => new FeelVariable(name));

    // context expression, exp: { key:expr }
    static readonly Parser<FeelExpression> _context =
        from _lb in Parse.Char('{').Token()
        from entries in (
            from key in Parse.Letter.AtLeastOnce().Text().Token()
            from _colon in Parse.Char(':').Token()
            from value in _add
            select new { key, value }).DelimitedBy(Parse.Char(',').Token())
        from _rb in Parse.Char('}').Token()
        select new FeelContext(entries.ToDictionary(e => e.key, e => e.value));

    // list, range and some
    static readonly Parser<FeelExpression> _list =
        from _lb in Parse.Char('[').Token()
        from items in _add.DelimitedBy(Parse.Char(',').Token())
        from _rb in Parse.Char(']').Token()
        select new FeelList(items);
    static readonly Parser<FeelExpression> _range = (
        from open in Parse.Char('[').Or(Parse.Char('(')).Token()
        from lb in _add.Token()
        from _dots in Parse.String("..").Token()
        from ub in _add.Token()
        from close in Parse.Char(']').Or(Parse.Char(')')).Token()
        select new FeelRange(lb, ub, open == '[', close == ']')).Token();

    // function call
    static readonly Parser<FeelExpression> _fnCall =
        from name in Parse.Letter.AtLeastOnce().Text().Token()
        from _lp in Parse.Char('(').Token()
        from args in _add.DelimitedBy(Parse.Char(',').Token()).Optional()
        from _rp in Parse.Char(')').Token()
        select (FeelExpression)new FeelFunctionCall(name, args.GetOrElse([]));

    // Parenthesized
    static readonly Parser<FeelExpression> _parn =
        from _lp in Parse.Char('(').Token()
        from expr in _logical
        from _rp in Parse.Char(')').Token()
        select expr;


    // ____________ 2. parser pipeline ___________//

    // primary: functions, parentheses, context, literals, identifiers
    static readonly Parser<FeelExpression> _primary =
        _fnCall
        .Or(_parn)
        .Or(_context)
        .Or(_range)
        .Or(_list)
        .Or(_null)
        .Or(_bool)
        .Or(_number)
        .Or(_string)
        .Or(_identifier);

    static readonly Parser<FeelExpression> _propertyAccess =
        from target in _primary
        from accesses in (
            from _ in Parse.Char('.').Token()
            from prop in Parse.Not(Parse.Char('.')).Then(_ =>
                Parse.Letter.Then(firstChar => Parse.LetterOrDigit.Many().Text().Select(rest => firstChar + rest)).Token()
            )
            select (target, prop)
        ).Many()
        select accesses.Aggregate(target, (current, access) => new FeelContextPropertyAccess(current, access.prop));

    static readonly Parser<FeelExpression> _base = _propertyAccess.Or(_primary);

    // Unary operators
    static readonly Parser<FeelExpression> _unary = (
        from ops in Parse.String("not").Token()
            .Or(Parse.Char('-').Select(_ => "-"))
            .Or(Parse.Char('+').Select(_ => "+")).Token().Text().Many()
        from term in _base
        select ops.Reverse().Aggregate(term, (expr, op) => new FeelUnary(op, expr))).Token();

    // Binary operators with proper precedence
    static readonly Parser<FeelExpression> _expo =
        Parse.ChainOperator(Parse.String("**").Token().Text(),
        _unary,
        (op, left, right) => new FeelBinary(left, op, right));
    static readonly Parser<FeelExpression> _mult =
        Parse.ChainOperator(Parse.Char('*').Or(Parse.Char('/')).Token().Select(c => c.ToString()),
        _expo,
        (op, left, right) => new FeelBinary(left, op, right));
    static readonly Parser<FeelExpression> _add =
        Parse.ChainOperator(Parse.Char('+').Or(Parse.Char('-')).Token().Select(c => c.ToString()),
        _mult,
        (op, left, right) => new FeelBinary(left, op, right));

    // if then else
    static readonly Parser<FeelExpression> _ifThenElse =
        from _if in Parse.String("if").Token()
        from condition in _logical
        from _then in Parse.String("then").Token()
        from thenExpr in _logical
        from _else in Parse.String("else").Token()
        from elseExpr in _logical
        select new FeelIfElse(condition, thenExpr, elseExpr);

    // some
    static readonly Parser<FeelExpression> _some =
        from _some in Parse.String("some").Token()
        from variable in _identifier.Select(v => ((FeelVariable)v).Name)
        from _in in Parse.String("in").Token()
        from collection in _list.Or(_range)
        from _satisfies in Parse.String("satisfies").Token()
        from condition in _logical
        select new FeelSome(variable, collection, condition);

    // instance of, exp: x instance of y
    static readonly Parser<FeelExpression> _instanceOf =
        from left in _base
        from _instanceOf in Parse.String("instance of").Token()
        from typeName in Parse.Letter.AtLeastOnce().Token().Text()
        select new FeelInstanceOf(left, typeName);

    //  between, exp: x between y and z
    static readonly Parser<FeelExpression> _between =
        from left in _base
        from _btw in Parse.String("between").Token()
        from lower in _base
        from _and in Parse.String("and").Token()
        from upper in _base
        select new FeelBetween(left, lower, upper);

    // in [x..y] 
    static readonly Parser<FeelExpression> _in =
        from val in _add
        from _word in Parse.String("in").Token()
        from coll in _base
        select new FeelIn(val, coll);

    static readonly Parser<FeelExpression> _cmp =
        _instanceOf
        .Or(_between)
        .Or(_some)
        .Or(_in)
        .Or(
            Parse.ChainOperator(Parse
                .String(">=")
                .Or(Parse.String("<="))
                .Or(Parse.String(">"))
                .Or(Parse.String("<"))
                .Or(Parse.String("="))
                .Or(Parse.String("!="))
                .Text()
                .Token(),
            _add,
            (op, left, right) => new FeelBinary(left, op, right))
        );
    static readonly Parser<FeelExpression> _logical =
        Parse.ChainOperator(Parse.String("and").Or(Parse.String("or")).Text().Token(),
        _cmp,
        (op, left, right) => new FeelBinary(left, op, right));


    //__________ top level (combine all) _________//
    static readonly Parser<FeelExpression> _fullExpr = _ifThenElse.Or(_logical);
}
