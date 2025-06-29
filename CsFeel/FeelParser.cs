using System.Globalization;
using Sprache;

namespace CsFeel;

public static class FeelParser
{
    public static readonly Parser<FeelExpression> Null =
        from _ in Parse.String("null").Token()
        select new FeelLiteral(null);

    public static readonly Parser<FeelExpression> Bool =
        from x in Parse.String("true").Or(Parse.String("false")).Token().Text()
        select new FeelLiteral(x == "true");

    private static readonly Parser<FeelExpression> Number =
        Parse.DecimalInvariant.Select(value => new FeelLiteral(
            decimal.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture)));

    private static readonly Parser<FeelExpression> String =
        from open in Parse.Char('"')
        from content in Parse.CharExcept('"').Many().Text()
        from close in Parse.Char('"')
        select new FeelLiteral(content);

    static readonly Parser<FeelExpression> Identifier =
        Parse.Identifier(Parse.Letter, Parse.LetterOrDigit).Select(name => new FeelVariable(name));

    static readonly Parser<FeelExpression> Arithmetic =
        from left in Identifier.Or(Number).Token()
        from op in Parse.String("+")
            .Or(Parse.String("-"))
            .Or(Parse.String("*"))
            .Or(Parse.String("/"))
            .Token()
            .Text()
        from right in Identifier.Or(Number).Token()
        select new FeelArithmetic(left, op, right);

    static readonly Parser<FeelExpression> Binary =
        from left in Identifier.Or(Number).Token()
        from op in Parse.String(">=")
            .Or(Parse.String("<="))
            .Or(Parse.String("="))
            .Or(Parse.String("!="))
            .Or(Parse.String(">"))
            .Or(Parse.String("<"))
            .Token()
            .Text()
        from right in Identifier.Or(Number).Token()
        select new FeelBoolComparison(left, op, right);

    static readonly Parser<FeelExpression> IfThenElse =
        from _if in Parse.String("if").Token()
        from cond in Binary
        from _then in Parse.String("then").Token()
        from thenExpr in String.Or(Identifier).Or(Number)
        from _else in Parse.String("else").Token()
        from elseExpr in String.Or(Identifier).Or(Number)
        select new FeelIfElse(cond, thenExpr, elseExpr);

    public static readonly Parser<FeelExpression> List =
        from lbrack in Parse.Char('[').Token()
        from items in Expr.DelimitedBy(Parse.Char(',').Token())
        from rbrack in Parse.Char(']').Token()
        select new FeelList(items);

    public static readonly Parser<FeelExpression> Range =
        from start in Number.Token()
        from dots in Parse.String("..").Token()
        from end in Number.Token()
        select new FeelRange(start, end);

    public static readonly Parser<FeelExpression> ListOrRange =
        Range.Or(List);

    public static readonly Parser<FeelExpression> SomeExpr =
        from _some in Parse.String("some").Token()
        from variable in Identifier.Select(v => ((FeelVariable)v).Name)
        from _in in Parse.String("in").Token()
        from collection in ListOrRange
        from _satisfies in Parse.String("satisfies").Token()
        from condition in Binary
        select new FeelSome(variable, collection, condition);

    public static readonly Parser<FeelExpression> Expr =
        IfThenElse
        .Or(ListOrRange)
        .Or(SomeExpr)
        .Or(Binary)
        .Or(Arithmetic)
        .Or(String)
        .Or(Number)
        .Or(Bool)
        .Or(Null)
        .Or(Identifier);


}
