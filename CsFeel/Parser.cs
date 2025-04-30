using CsFeel.Internals;
using CsFeel.Internals.Nodes;
using CsFeel.Internals.Exceptions;

namespace CsFeel;

public class Parser(Tokenizer tokenizer)
{
    protected readonly Tokenizer _tokenizer = tokenizer;

    public Node<decimal> ParseExpression()
    {
        var node = ParseArithmetic();

        return node;
    }

    protected Node<decimal> ParseArithmetic()
    {
        _tokenizer.NextToken();

        var lhs = ParseLeaf();

        while (true)
        {
            _tokenizer.NextToken();

            Func<decimal, decimal, decimal>? op = null;
            if (_tokenizer.CurrentToken == Token.ADD)
            {
                op = (a, b) => a + b;
            }
            if (_tokenizer.CurrentToken == Token.SUB)
            {
                op = (a, b) => a - b;
            }
            if (_tokenizer.CurrentToken == Token.MUL)
            {
                op = (a, b) => a * b;
            }
            if (_tokenizer.CurrentToken == Token.DIV)
            {
                op = (a, b) => a / b;
            }
            if (_tokenizer.CurrentToken == Token.EXP)
            {
                op = (a, b) => (decimal)Math.Pow((double)a, (double)b);
            }

            if (op == null)
            {
                return lhs;
            }

            _tokenizer.NextToken();

            var rhs = ParseLeaf();

            lhs = new NodeNumeric(lhs, rhs, op);
        }
    }

    public Node<decimal> ParseLeaf()
    {
        if (_tokenizer.CurrentToken == Token.NUM)
        {
            var value = (decimal?)_tokenizer.CurrentTokenValue;
            if (value.HasValue)
            {
                return new NodeDecimal(value.Value);
            }
            else
            {
                throw new Exception($"null value at token: {_tokenizer.CurrentToken}");
            }
        }
        else
        {
            throw new Exception($"can't parse token:{_tokenizer.CurrentToken}");
        }
    }
}