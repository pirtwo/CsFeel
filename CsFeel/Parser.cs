using CsFeel.Internals;
using CsFeel.Internals.Nodes;
using CsFeel.Internals.Exceptions;

namespace CsFeel;

public class Parser(Tokenizer tokenizer)
{
    protected readonly IContext _context = new Context();
    protected readonly Tokenizer _tokenizer = tokenizer;

    public INode<decimal> ParseExpression()
    {
        var node = ParseAddSub();

        return node;
    }

    /*--- parse add and sub ---*/
    protected INode<decimal> ParseAddSub()
    {
        var lhs = ParseMulDiv();

        while (true)
        {
            Func<decimal, decimal, decimal>? op = null;
            if (_tokenizer.CurrentToken == Token.ADD)
            {
                op = (a, b) => a + b;
            }
            if (_tokenizer.CurrentToken == Token.SUB)
            {
                op = (a, b) => a - b;
            }

            if (op == null)
            {
                return lhs;
            }

            // skip + or -
            _tokenizer.NextToken();

            var rhs = ParseMulDiv();

            lhs = new NodeArithmetic(lhs, rhs, op);
        }
    }

    /*--- parse mul and div*/
    protected INode<decimal> ParseMulDiv()
    {
        var lhs = ParseExp();

        while (true)
        {
            Func<decimal, decimal, decimal>? op = null;
            if (_tokenizer.CurrentToken == Token.MUL)
            {
                op = (a, b) => a * b;
            }
            if (_tokenizer.CurrentToken == Token.DIV)
            {
                op = (a, b) => a / b;
            }

            if (op == null)
            {
                return lhs;
            }

            // skip * or /
            _tokenizer.NextToken();

            var rhs = ParseExp();

            lhs = new NodeArithmetic(lhs, rhs, op);
        }
    }

    /*--- parse exponent ---*/
    protected INode<decimal> ParseExp()
    {
        var lhs = ParseUnary();

        while (true)
        {
            Func<decimal, decimal, decimal>? op = null;
            if (_tokenizer.CurrentToken == Token.EXP)
            {
                op = (a, b) => (decimal)Math.Pow((double)a, (double)b);
            }

            if (op == null)
            {
                return lhs;
            }

            // skip **
            _tokenizer.NextToken();

            var rhs = ParseUnary();

            lhs = new NodeArithmetic(lhs, rhs, op);
        }
    }

    /*--- parse unary, exp: !x ---*/
    protected INode<decimal> ParseUnary()
    {
        if (_tokenizer.CurrentToken == Token.ADD)
        {
            // skip
            _tokenizer.NextToken();

            return ParseUnary();
        }

        if (_tokenizer.CurrentToken == Token.SUB)
        {
            // skip
            _tokenizer.NextToken();

            var rhs = ParseUnary();

            return new NodeUnary(rhs, (x) => -x);
        }

        return ParseLeaf();
    }

    /*--- parse num, par, var ---*/
    protected INode<decimal> ParseLeaf()
    {
        // is number ?
        if (_tokenizer.CurrentToken == Token.NUM)
        {
            var value = (decimal?)_tokenizer.CurrentTokenValue;
            if (value.HasValue)
            {
                _tokenizer.NextToken();
                return new NodeNumber(value.Value);
            }
            else
            {
                throw new ParserException(
                    ParserExceptionType.INVALID_TOKEN_VALUE,
                    $"null value at token: {_tokenizer.CurrentToken}");
            }
        }

        // is parenthesis ?
        if (_tokenizer.CurrentToken == Token.PAR_OPE)
        {
            // skip '('
            _tokenizer.NextToken();

            var n = ParseAddSub();

            if (_tokenizer.CurrentToken != Token.PAR_CLO)
            {
                throw new ParserException(
                    ParserExceptionType.SYNTAX_ERROR,
                    $"invalid char:{(string?)_tokenizer.CurrentTokenValue}, expected: ')'");
            }

            // skip ')'
            _tokenizer.NextToken();

            return n;
        }

        // is variable ?
        // if (_tokenizer.CurrentToken == Token.VAR)
        // {
        //     string varName = _tokenizer.CurrentTokenValue?.ToString()
        //         ?? throw new ParserException(
        //             ParserExceptionType.NULL_VARIABLE_NAME,
        //             "can't resolve variable name");

        //     NodeVariable n = new(_context, _tokenizer.CurrentTokenValue.ToString());

        //     return n;
        // }

        throw new ParserException(
            ParserExceptionType.INVALID_TOKEN,
            $"can't parse token:{_tokenizer.CurrentToken}");
    }
}