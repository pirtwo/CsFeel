using Sprache;

namespace CsFeel.Test.ParserTests;

public class BooleanExpression
{
    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    public void LiteralTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("null = null", true)]
    [InlineData("null != null", false)]
    [InlineData("5 = 5", true)]
    [InlineData("4 != 4", false)]
    [InlineData("2.5 != 2.5", false)]
    [InlineData("-2.5 = 2.5", false)]
    [InlineData("3 > 2", true)]
    [InlineData("3 >= 3", true)]
    [InlineData("3 < 2", false)]
    [InlineData("3 <= 3", true)]
    [InlineData("2.5 <= 2.3", false)]
    [InlineData("5 between 3 and 6", true)]
    [InlineData("5 between 3 and 6 = true", true)]
    public void ComparisonTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("null = null", true)]
    [InlineData("\"foo\" = null", false)]
    [InlineData("{x: null}.x = null", true)]
    [InlineData("{}.y = null", true)]
    public void NullCheckTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("true and true", true)]
    [InlineData("true and false", false)]
    [InlineData("false and null", false)]
    [InlineData("false and \"otherwise\"", false)]
    public void AndTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("true and null")]
    [InlineData("true and \"otherwise\"")]
    public void NullAndTest(string input)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("true or true", true)]
    [InlineData("true or false", true)]
    [InlineData("true or null", true)]
    [InlineData("true or \"otherwise\"", true)]
    public void OrTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }
    [Theory]
    [InlineData("false or null")]
    [InlineData("false or \"otherwise\"")]
    public void NullOrTest(string input)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("1 instance of boolean", false)]
    [InlineData("true instance of boolean", true)]
    [InlineData("1.99 instance of number", true)]
    [InlineData("null instance of number", false)]
    [InlineData("\"hello\" instance of string", true)]
    [InlineData("1 instance of string", false)]
    [InlineData("date(\"2000-01-01\") instance of date", true)]
    [InlineData("1 instance of date", false)]
    [InlineData("time(\"14:20:32\") instance of time", true)]
    [InlineData("1 instance of time", false)]
    [InlineData("[1,2,3] instance of list", true)]
    [InlineData("0 instance of list", false)]
    [InlineData("1 instance of context", false)]
    [InlineData("1 instance of any", true)]
    public void InstanceOfTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Equal(expected, result);
    }

}