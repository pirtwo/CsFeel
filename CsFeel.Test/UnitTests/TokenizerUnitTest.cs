using CsFeel.Internals;

namespace CsFeel.Test.UnitTests;

public class TokenizerUnitTest
{
    [Theory]
    [InlineData("", Token.EOF)]
    [InlineData("+", Token.ADD)]
    [InlineData("-", Token.SUB)]
    [InlineData("*", Token.MUL)]
    [InlineData("/", Token.DIV)]
    [InlineData("**", Token.EXP)]
    [InlineData("001", Token.NUM)]
    [InlineData("22.55", Token.NUM)]
    [InlineData(".98", Token.NUM)]
    public void TokenDetectionTest(string input, Token expectedToken)
    {
        // Arrange
        var tokenizer = new Tokenizer(new StringReader(input));

        // Act
        tokenizer.NextToken();

        // Assert
        Assert.Equal(expectedToken, tokenizer.CurrentToken);
    }
}