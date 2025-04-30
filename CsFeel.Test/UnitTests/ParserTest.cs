using CsFeel.Internals;

namespace CsFeel.Test.UnitTests;

public class ParserTest
{
    [Theory]
    [InlineData("1", 1)]
    [InlineData("0.5", 0.5)]
    [InlineData(".5", 0.5)]
    [InlineData("-2", -2)]
    [InlineData("01", 1)]
    [InlineData("-0002", -2)]
    public void TestCase001(string expression, decimal expected)
    {
        // arrange
        Tokenizer t = new(new StringReader(expression));
        Parser p = new(t);

        // act
        var result = p.ParseExpression().Eval();

        // assert
        Assert.Equal(expected, result);
    }
}