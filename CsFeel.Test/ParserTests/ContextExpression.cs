using Sprache;

namespace CsFeel.Test.ParserTests;

public class ContextExpression
{
    [Fact]
    public void DeclerationTest()
    {
        // arrange
        var exp = FeelParser.Expr.Parse("{a:1, b:\"hello\"}");

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.IsType<Dictionary<string, object?>>(result);
        Assert.True(((Dictionary<string, object?>)result).ContainsKey("a"));
        Assert.True(((Dictionary<string, object?>)result).ContainsKey("b"));
        Assert.Equal(1, (decimal)((Dictionary<string, object?>)result)["a"]!);
        Assert.Equal("hello", (string)((Dictionary<string, object?>)result)["b"]!);
    }

    [Fact]
    public void NestedTest()
    {
        // arrange
        var exp = FeelParser.Expr.Parse("{a:1, b:\"hello\", c:{x:12, y:null}}");

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.IsType<Dictionary<string, object?>>(result);
        Assert.True(((Dictionary<string, object?>)result).ContainsKey("a"));
        Assert.True(((Dictionary<string, object?>)result).ContainsKey("b"));
        Assert.True(((Dictionary<string, object?>)result).ContainsKey("c"));

        Assert.Equal(1, (decimal)((Dictionary<string, object?>)result)["a"]!);
        Assert.Equal("hello", (string)((Dictionary<string, object?>)result)["b"]!);
        Assert.IsType<Dictionary<string, object?>>(((Dictionary<string, object?>)result)["c"]);
    }

    [Fact]
    public void PropertyAccessTest()
    {
        // arrange
        var exp = FeelParser.Expr.Parse("{a:1, b:\"hello\"}.b");

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Equal("hello", result);
    }

    [Fact]
    public void NestedPropertyAccessTest()
    {
        // arrange
        var exp = FeelParser.Expr.Parse("{point:{x:1,y:2}}.point.x");

        // act
        var result = FeelExpressionEval.Eval(exp, []);

        // assert
        Assert.Equal(1m, (decimal)result!);
    }
}