using CsFeel.Internals;

namespace CsFeel.Test.ParserTests;

public class ListExpression
{
    [Fact]
    public void LiteralTest()
    {
        // arrange
        Tokenizer t = new(new StringReader("[1,2,3]"));
        Parser p = new(t);

        // act
        var result = p.ParseExpression();

        // assert
        Assert.IsType<List<decimal>>(result?.GetType());
        Assert.Collection(
            (List<decimal>)result,
            e => { Assert.Equal(1, e); },
            e => { Assert.Equal(2, e); },
            e => { Assert.Equal(3, e); });
    }
}