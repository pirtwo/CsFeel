using Sprache;
using CsFeel.Evaluators;

namespace CsFeel.Test.ParserTests;

public class ControlFlow
{
    [Theory]
    [InlineData("if 1 > 2 then true else false", false)]
    [InlineData("if 1 < 2 then true else false", true)]
    [InlineData("if 1 = 1 then true else false", true)]
    public void ControlFlowTest(string input, bool expected)
    {
        // arrange
        var exp = FeelParser.Expr.Parse(input);

        // act
        var result = FeelExpressionEvaluator.Eval(exp, []);

        // assert
        Assert.Equal(expected, (bool)result!);
    }
}