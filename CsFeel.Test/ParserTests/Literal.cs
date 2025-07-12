using System.Globalization;
using CsFeel.Evaluators;
using Sprache;

namespace CsFeel.Test.ParserTests;

public class Literal
{
    [Theory]
    [InlineData("null")]
    public void NullTest(string input)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("2.3", 2.3)]
    [InlineData(".3", .3)]
    [InlineData("-5", -5)]
    public void NumberTest(string input, decimal expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("\"hello\"", "hello")]
    public void StringTest(string input, string expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void BoolTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("date(\"2000-01-01\")", "2000-01-01")]
    public void DateTest(string input, string expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(DateTime.Parse(expected), result);
    }

    [Fact]
    public void DateNullTest()
    {
        // arrange
        var exp = FeelParser.Expr.Parse("date(11111)");

        // act 
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Null(result);
    }

    [Fact]
    public void ListTest()
    {
        // arrange
        var exp = FeelParser.Expr.Parse("[1,\"hello\",null]");

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.IsType<List<object?>>(result);
        Assert.Collection(
            (List<object?>)result,
            e => { Assert.Equivalent(1, e); },
            e => { Assert.Equivalent("hello", e); },
            e => { Assert.Equivalent(null, e); });
    }

    [Fact]
    public void NestedListTest()
    {
        // arrange
        var exp = FeelParser.Expr.Parse("[12,22,[12,9]]");

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.IsType<List<object?>>(result);
        Assert.Collection(
            (List<object?>)result,
            e => { Assert.Equivalent(12, e); },
            e => { Assert.Equivalent(22, e); },
            e => { Assert.IsType<List<object?>>(e); });
    }
}