using CsFeel.Internals;
using CsFeel.Internals.Nodes;

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
        var result = (List<INode>)p.ParseExpression()!;
        var list = result.Select(x =>
        {
            var e = new Evaluator();

            x.Accept(e);

            return (decimal)e.GetResult()!;
        }).ToList();

        // assert
        Assert.IsType<List<decimal>>(list);
        Assert.Collection(
            list,
            e => { Assert.Equal(1, e); },
            e => { Assert.Equal(2, e); },
            e => { Assert.Equal(3, e); });
    }
}