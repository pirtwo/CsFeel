using CsFeel.Internals;

namespace CsFeel.Test.UnitTests;

public class TokenizerTest
{
    [Theory]
    [InlineData("", Token.EOF)]
    [InlineData("+", Token.ADD)]
    [InlineData("-", Token.SUB)]
    [InlineData("*", Token.MUL)]
    [InlineData("/", Token.DIV)]
    [InlineData("**", Token.POW)]
    [InlineData("001", Token.NUM)]
    [InlineData("22.55", Token.NUM)]
    [InlineData(".98", Token.NUM)]
    public void DetectionTest(string input, Token expectedToken)
    {
        // Arrange
        // Act
        var tokenizer = new Tokenizer(new StringReader(input));

        // Assert
        Assert.Equal(expectedToken, tokenizer.CurrentToken);
    }
}