// using CsFeel.Internals;

// namespace CsFeel.Test.ParserTests;

// public class ContextExpression
// {
//     [Theory]
//     [InlineData("{{a:1, b:2}}")]
//     public void LiteralTest(string expression)
//     {
//         // arrange
//         Tokenizer t = new(new StringReader(expression));
//         Parser p = new(t);

//         // act
//         var result = p.ParseExpression();

//         // assert
//         Assert.Equal(new { a = 1, b = 2 }, result);
//     }
// }