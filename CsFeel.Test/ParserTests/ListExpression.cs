using CsFeel.Evaluators;
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
        var result = FeelExpressionEvaluator.Eval(exp, []);

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
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (bool)result!);
    }

    [Theory]
    [InlineData("list contains([1,2,3], 1) = true")]
    [InlineData("list contains([1,2,3], 4) = false")]
    [InlineData("list contains([1,2,3,null], null) = true")]
    [InlineData("list replace([1,2,3,4], 5, 0) = null")]
    [InlineData("list replace([1,2,3,4], 0, 0) = null")]
    [InlineData("list replace([1,2,3,4], -9, 0) = null")]
    [InlineData("list replace([1,2,3,4], -1, 0) = null")]
    [InlineData("list replace([1,2,3,4], 1.1, 0) = null")]
    [InlineData("list replace([1,2,3,4], 1, 0) = [0,2,3,4]")]
    [InlineData("list replace([1,2,3,4], 2, 0) = [1,0,3,4]")]
    [InlineData("list replace([1,2,3,4], 3, 0) = [1,2,0,4]")]
    [InlineData("list replace([1,2,3,4], 4, 0) = [1,2,3,0]")]
    [InlineData("count(2) = null")]
    [InlineData("count([]) = 0")]
    [InlineData("count([\"x\",2,3]) = 3")]
    [InlineData("count([1,2.1,3,null]) = 4")]
    [InlineData("count([1,2.1,3,[1,2]]) = 4")]
    [InlineData("min( [] ) = null")]
    [InlineData("min( [1,2,3] ) = 1")]
    [InlineData("max( [] ) = null")]
    [InlineData("max( [1,2,3] ) = 3")]
    [InlineData("sum( [] ) = null")]
    [InlineData("sum( [1,2,3] ) = 6")]
    [InlineData("mean( [] ) = null")]
    [InlineData("mean( [1,2,3] ) = 2")]
    [InlineData("all( [false,null,true] ) = false")]
    [InlineData("all( true ) = true")]
    [InlineData("all( [true] ) = true")]
    [InlineData("all( [] ) = true")]
    [InlineData("all( 0 ) = null")]
    [InlineData("any( [false,null,true] ) = true")]
    [InlineData("any( false ) = false")]
    [InlineData("any( [true] ) = true")]
    [InlineData("any( [] ) = false")]
    [InlineData("any( 0 ) = null")]
    [InlineData("median( [ ] ) = null")]
    [InlineData("median( [6, 1, 2, 3] ) = 2.5")]
    [InlineData("append( [1], 2, 3 ) = [1,2,3]")]
    [InlineData("reverse( [1,2,3] ) = [3,2,1]")]
    [InlineData("index of( [1,2,3,2],2 ) = [2,4]")]
    public void ListFnTest(string input)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.True((bool)result!);
    }

}