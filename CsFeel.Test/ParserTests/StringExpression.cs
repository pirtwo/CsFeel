using CsFeel.Evaluators;
using Sprache;

namespace CsFeel.Test.ParserTests;

public class StringExpression
{
    [Theory]
    [InlineData("\"hello world!!!\"", "hello world!!!")]
    public void LiteralTest(string input, string expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("\"foo\" + \"bar\"", "foobar")]
    [InlineData("\"foo\" + string(123)", "foo123")]
    public void ConcatTest(string input, string expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }
}