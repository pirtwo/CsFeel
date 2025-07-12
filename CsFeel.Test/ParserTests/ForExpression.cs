using CsFeel.Evaluators;
using Sprache;

namespace CsFeel.Test.ParserTests;

public class ForExpression
{
    [Fact]
    public void ForTest()
    {
        // arrange
        var exp = FeelParser.Expr.Parse("for x in [1,2,3,4] return x*2");

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.IsType<List<object?>>(result);
        Assert.Collection(
            (List<object?>)result,
            e => { Assert.Equivalent(2, e); },
            e => { Assert.Equivalent(4, e); },
            e => { Assert.Equivalent(6, e); },
            e => { Assert.Equivalent(8, e); });
    }
}