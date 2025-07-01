using System.Globalization;
using Sprache;

namespace CsFeel;

public static class FeelParser
{
    public static readonly Parser<FeelExpression> Null =
        from _ in Parse
            .String("null")
            .Token()
        select new FeelLiteral(null);

    public static readonly Parser<FeelExpression> Bool =
        from x in Parse
            .String("true")
            .Or(Parse.String("false"))
            .Token()
            .Text()
        select new FeelLiteral(x == "true");

    public static readonly Parser<FeelExpression> Number =
        Parse
        .DecimalInvariant
        .Select(value => new FeelLiteral(
            decimal.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture)));

    public static readonly Parser<FeelExpression> String =
        from open in Parse
            .Char('"')
        from content in Parse
            .CharExcept('"')
            .Many()
            .Text()
        from close in Parse
            .Char('"')
        select new FeelLiteral(content);

    public static readonly Parser<FeelExpression> Identifier =
        Parse
        .Identifier(Parse.Letter, Parse.LetterOrDigit)
        .Select(name => new FeelVariable(name));

    public static readonly Parser<FeelExpression> Parenthesized =
        from lparen in Parse
            .Char('(')
            .Token()
        from expr in Expr
        from rparen in Parse
            .Char(')')
            .Token()
        select expr;

    public static readonly Parser<FeelExpression> IfThenElse =
        from _if in Parse
            .String("if")
            .Token()
        from condition in Expr
        from _then in Parse
            .String("then")
            .Token()
        from thenExpr in Expr
        from _else in Parse
            .String("else")
            .Token()
        from elseExpr in Expr
        select new FeelIfElse(condition, thenExpr, elseExpr);

    public static readonly Parser<FeelExpression> List =
        from lbrack in Parse
            .Char('[')
            .Token()
        from items in Expr
            .DelimitedBy(Parse.Char(',').Token())
        from rbrack in Parse
            .Char(']')
            .Token()
        select new FeelList(items);

    public static readonly Parser<FeelExpression> Range =
        from start in Number
            .Token()
        from dots in Parse
            .String("..")
            .Token()
        from end in Number
            .Token()
        select new FeelRange(start, end);

    public static readonly Parser<FeelExpression> ListOrRange =
        Range.Or(List);

    public static readonly Parser<FeelExpression> Some =
        from _some in Parse
            .String("some")
            .Token()
        from variable in Identifier.Select(v => ((FeelVariable)v).Name)
        from _in in Parse
            .String("in")
            .Token()
        from collection in ListOrRange
        from _satisfies in Parse
            .String("satisfies")
            .Token()
        from condition in Expr
        select new FeelSome(variable, collection, condition);

    public static readonly Parser<FeelExpression> FunctionCall =
        from name in Parse
            .Letter
            .Or(Parse.Char(' '))
            .AtLeastOnce()
            .Text()
            .Token()
        from lparen in Parse
            .Char('(')
            .Token()
        from args in Expr.DelimitedBy(Parse.Char(',').Token())
        from rparen in Parse
            .Char(')')
            .Token()
        select new FeelFunctionCall(name.Trim(), args);

    public static readonly Parser<FeelExpression> Unary =
        from ops in
            Parse
            .Char('+').Select(c => c.ToString())
            .Or(Parse.Char('-').Select(c => c.ToString()))
            .Token()
            .Text()
            .Many()
        from operand in
            Parenthesized
            .Or(FunctionCall)
            .Or(ListOrRange)
            .Or(Null)
            .Or(Bool)
            .Or(Number)
            .Or(String)
            .Or(Identifier)
        select ops
            .Reverse()
            .Aggregate(operand, (acc, op) => new FeelUnary(op, acc));

    public static readonly Parser<FeelExpression> Multiplicative =
        Parse.ChainOperator(
            Parse
                .Char('*')
                .Or(Parse.Char('/'))
                .Token()
                .Select(c => c.ToString()),
            Unary,
            (op, left, right) => new FeelBinary(left, op, right)
        );

    public static readonly Parser<FeelExpression> Additive =
        Parse.ChainOperator(
            Parse
                .Char('+')
                .Or(Parse.Char('-'))
                .Token()
                .Select(c => c.ToString()),
            Multiplicative,
            (op, left, right) => new FeelBinary(left, op, right)
        );

    public static readonly Parser<FeelExpression> Comparison =
        Parse.ChainOperator(Parse
                .String(">=")
                .Or(Parse.String("<="))
                .Or(Parse.String(">"))
                .Or(Parse.String("<"))
                .Or(Parse.String("="))
                .Or(Parse.String("!="))
                .Text()
                .Token(),
            Additive,
            (op, left, right) => new FeelBinary(left, op, right)
        );

    public static readonly Parser<FeelExpression> Logical =
        Parse.ChainOperator(
            Parse
                .String("and")
                .Or(Parse.String("or"))
                .Text()
                .Token(),
            Comparison,
            (op, left, right) => new FeelBinary(left, op, right)
        );

    public static readonly Parser<FeelExpression> Expr = IfThenElse.Or(Some).Or(Logical);
}
