// using CsFeel;

// namespace CsFeel.Test.ParserTests;

// public class NumericExpression
// {
//     [Theory]
//     [InlineData("1", 1)]
//     [InlineData("2.3", 2.3)]
//     [InlineData(".3", .3)]
//     [InlineData("-5", -5)]
//     public void LiteralTest(string expression, decimal expected)
//     {
//         // arrange
//         Tokenizer t = new(new StringReader(expression));
//         Parser p = new(t);

//         // act
//         var result = p.ParseExpression();

//         // assert
//         Assert.Equal(expected, result);
//     }

//     [Theory]
//     [InlineData("2 + 3", 5)]
//     public void AddTest(string expression, decimal expected)
//     {
//         // arrange
//         Tokenizer t = new(new StringReader(expression));
//         Parser p = new(t);

//         // act
//         var result = p.ParseExpression();

//         // assert
//         Assert.Equal(expected, result);
//     }

//     [Theory]
//     [InlineData("22 - 3", 19)]
//     public void SubTest(string expression, decimal expected)
//     {
//         // arrange
//         Tokenizer t = new(new StringReader(expression));
//         Parser p = new(t);

//         // act
//         var result = p.ParseExpression();

//         // assert
//         Assert.Equal(expected, result);
//     }

//     [Theory]
//     [InlineData("3 * 55", 165)]
//     public void MulTest(string expression, decimal expected)
//     {
//         // arrange
//         Tokenizer t = new(new StringReader(expression));
//         Parser p = new(t);

//         // act
//         var result = p.ParseExpression();

//         // assert
//         Assert.Equal(expected, result);
//     }

//     [Theory]
//     [InlineData("4 / 2", 2)]
//     public void DivTest(string expression, decimal expected)
//     {
//         // arrange
//         Tokenizer t = new(new StringReader(expression));
//         Parser p = new(t);

//         // act
//         var result = p.ParseExpression();

//         // assert
//         Assert.Equal(expected, result);
//     }

//     [Theory]
//     [InlineData("2 ** 3", 8)]
//     public void ExpoTest(string expression, decimal expected)
//     {
//         // arrange
//         Tokenizer t = new(new StringReader(expression));
//         Parser p = new(t);

//         // act
//         var result = p.ParseExpression();

//         // assert
//         Assert.Equal(expected, result);
//     }
// }