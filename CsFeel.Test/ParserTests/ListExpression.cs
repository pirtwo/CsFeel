using Sprache;

namespace CsFeel.Test.ParserTests;

public class ListExpression
{
    [Theory]
    [InlineData("some x in [1,2,3,4] satisfies x > 2", true)]
    [InlineData("some x in [1,2,3,4] satisfies x > 11", false)]
    [InlineData("some x in [1,-2,3,4] satisfies x < 0", true)]
    [InlineData("some x in [1,-2,3,4] satisfies x = 5", false)]
    public void SomeTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Equal(expected, (bool)result!);
    }

    [Theory]
    [InlineData("5 in [1..10]", true)]
    [InlineData("5 in [1..5)", false)]
    [InlineData("1 in [1..5]", true)]
    [InlineData("1 in (1..5]", false)]
    [InlineData("-1 in [-2..0]", true)]
    [InlineData("-1 in [0..5]", false)]
    [InlineData("11 in [1..10]", false)]
    [InlineData("3 in [1,1,3]", true)]
    [InlineData("2 in [1,3,4]", false)]
    public void RangeTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Equal(expected, (bool)result!);
    }

}