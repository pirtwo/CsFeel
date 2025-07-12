using CsFeel.Evaluators;
using Sprache;

namespace CsFeel.Test.ParserTests;

public class NumericExpression
{
    [Theory]
    [InlineData("1", 1)]
    [InlineData("2.3", 2.3)]
    [InlineData(".3", .3)]
    [InlineData("-5", -5)]
    [InlineData("--++--2", 2)]
    [InlineData("--+---2", -2)]
    public void LiteralTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("2 + 3", 5)]
    public void AddTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("22 - 3", 19)]
    public void SubTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("3 * 55", 165)]
    public void MulTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("4 / 2", 2)]
    public void DivTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("2 ** 3", 8)]
    public void ExpoTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("-(2 + 3)", -5)]
    [InlineData("-2 + 3 * 3", 7)]
    [InlineData("(2 + 3) * 2 ** 3", 40)]
    public void OperationOrderTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }
}