using System.Globalization;
using Sprache;

namespace CsFeel.Test.ParserTests;

public class Literal
{
    [Theory]
    [InlineData("null")]
    public void NullLiteralTest(string input)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("2.3", 2.3)]
    [InlineData(".3", .3)]
    [InlineData("-5", -5)]
    public void NumberLiteralTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("\"hello\"", "hello")]
    public void StringLiteralTest(string input, string expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void BoolLiteralTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }
}