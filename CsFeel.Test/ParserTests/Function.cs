using Sprache;
using CsFeel.Evaluators;

namespace CsFeel.Test.ParserTests;

public class Function
{
    [Theory]
    [InlineData("substring( \"testing\", 1)", "testing")]
    [InlineData("substring( \"testing\", 7)", "g")]
    [InlineData("substring( \"testing\", 8)", "")]
    [InlineData("substring( \"testing\", 0)", "g")]
    [InlineData("substring( \"testing\", 3)", "sting")]
    [InlineData("substring( \"testing\", 3, 3)", "sti")]
    [InlineData("substring( \"testing\", -2, 1)", "n")]
    public void FnSubstringTest(string input, string expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (string)result!);
    }

    [Theory]
    [InlineData("string length( \"test\" ) = 4", true)]
    public void FnStringLengthTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (bool)result!);
    }

    [Theory]
    [InlineData("upper case( \"test4\" )", "TEST4")]
    public void FnUpperCaseTest(string input, string expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (string)result!);
    }

    [Theory]
    [InlineData("lower case( \"TeSt4\" )", "test4")]
    public void FnLowerCaseTest(string input, string expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (string)result!);
    }

    [Theory]
    [InlineData("contains( \"TeSt4\", \"St\" )", true)]
    [InlineData("contains( \"TeSt4\", \"es\" )", false)]
    public void FnContainsTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (bool)result!);
    }


    //_____ number fn

    [Theory]
    [InlineData("floor(1.5)", 1)]
    [InlineData("floor(-1.5)", -2)]
    public void FnFloorTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (decimal)result!);
    }

    [Theory]
    [InlineData("ceiling(1.5)", 2)]
    [InlineData("ceiling(-1.5)", -1)]
    public void FnCeilingTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (decimal)result!);
    }

    [Theory]
    [InlineData("abs(1)", 1)]
    [InlineData("abs(-1)", 1)]
    public void FnAbsTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (decimal)result!);
    }

    [Theory]
    [InlineData("modulo(12,5)", 2)]
    [InlineData("modulo(-12,5)", 3)]
    [InlineData("modulo(12,-5)", -3)]
    [InlineData("modulo(-12,-5)", -2)]
    [InlineData("modulo(10.1,4.5)", 1.1)]
    [InlineData("modulo( -10.1, 4.5 )", 3.4)]
    [InlineData("modulo( 10.1, -4.5 )", -3.4)]
    [InlineData("modulo( -10.1, -4.5 )", -1.1)]
    public void FnModuloTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (decimal)result!);
    }

    [Theory]
    [InlineData("odd(5)", true)]
    [InlineData("odd(4)", false)]
    public void FnOddTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (bool)result!);
    }

    [Theory]
    [InlineData("even(5)", false)]
    [InlineData("even(4)", true)]
    public void FnEvenTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (bool)result!);
    }

    [Theory]
    [InlineData("sqrt(16)", 4)]
    [InlineData("sqrt(25)", 5)]
    public void FnSqrtTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (decimal)result!);
    }
}