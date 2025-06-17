using CsFeel.Internals;

namespace CsFeel.Test.ParserTests;

public class StringExpression
{
    [Theory]
    [InlineData("\"hello world!!!\"", "hello world!!!")]
    public void LiteralTest(string expression, string expected)
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
    [InlineData("\"foo\" + \"bar\"", "foobar")]
    [InlineData("\"foo\" + string(123)", "foo123")]
    public void AdditionTest(string expression, string expected)
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