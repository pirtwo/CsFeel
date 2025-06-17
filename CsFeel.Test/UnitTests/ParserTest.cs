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
    [InlineData("--+-+--3", -3)]
    public void LiteralsTest(string expression, decimal expected)
    {
        // arrange
        Tokenizer t = new(new StringReader(expression));
        Parser p = new(t);

        // act
        var result = p.ParseExpression();

        // assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("1 + 1 + 1", 3)]
    [InlineData("1 + 2 * 3", 7)]
    [InlineData("2 * 3 - 1", 5)]
    [InlineData("1 + 4 / 2", 3)]
    [InlineData("1 + 2 * 4 ** 2", 33)]
    [InlineData("(1+2)*3", 9)]
    public void OperationPrecedenceTest(string expression, decimal expected)
    {
        // arrange
        Tokenizer t = new(new StringReader(expression));
        Parser p = new(t);

        // act
        var result = p.ParseExpression();

        // assert
        Assert.Equal(expected, result);
    }

}