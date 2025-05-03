using CsFeel.Internals;
using CsFeel.Internals.Nodes;
using CsFeel.Internals.Exceptions;

namespace CsFeel;

public class Parser(Tokenizer tokenizer)
{
    protected readonly INodeVisitor _evaluator = new Evaluator();
    protected readonly IContext _context = new Context();
    protected readonly Tokenizer _tokenizer = tokenizer;

    public object? ParseExpression()
    {
        var node = ParseAddSub();

        node.Accept(_evaluator);

        return _evaluator.GetResult();
    }

    /*--- parse add and sub ---*/
    protected INode ParseAddSub()
    {
        var lhs = ParseMulDiv();

        while (true)
        {
            if (_tokenizer.CurrentToken == Token.ADD || _tokenizer.CurrentToken == Token.SUB)
            {
                var op = _tokenizer.CurrentToken ?? throw new Exception("");

                // skip + or -
                _tokenizer.NextToken();

                var rhs = ParseMulDiv();

                lhs = new NodeArithmetic(lhs, rhs, op);
            }
            else
            {
                return lhs;
            }
        }
    }

    /*--- parse mul and div*/
    protected INode ParseMulDiv()
    {
        var lhs = ParseExp();

        while (true)
        {
            if (_tokenizer.CurrentToken == Token.MUL || _tokenizer.CurrentToken == Token.DIV)
            {
                var op = _tokenizer.CurrentToken ?? throw new Exception("");

                // skip * or /
                _tokenizer.NextToken();

                var rhs = ParseExp();

                lhs = new NodeArithmetic(lhs, rhs, op);
            }
            else
            {
                return lhs;
            }
        }
    }

    /*--- parse exponent ---*/
    protected INode ParseExp()
    {
        var lhs = ParseUnary();

        while (true)
        {
            if (_tokenizer.CurrentToken == Token.POW)
            {
                var op = _tokenizer.CurrentToken ?? throw new Exception("");

                // skip **
                _tokenizer.NextToken();

                var rhs = ParseUnary();

                lhs = new NodeArithmetic(lhs, rhs, op);
            }
            else
            {
                return lhs;
            }
        }
    }

    /*--- parse unary, exp: !x ---*/
    protected INode ParseUnary()
    {
        if (_tokenizer.CurrentToken == Token.ADD)
        {
            // skip
            _tokenizer.NextToken();

            return ParseUnary();
        }

        if (_tokenizer.CurrentToken == Token.SUB)
        {
            var op = _tokenizer.CurrentToken ?? throw new Exception("");

            // skip
            _tokenizer.NextToken();

            var rhs = ParseUnary();

            return new NodeUnary(rhs, op);
        }

        return ParseLeaf();
    }

    /*--- parse num, par, var ---*/
    protected INode ParseLeaf()
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
        if (_tokenizer.CurrentToken == Token.VAR)
        {
            NodeVariable n = new(_tokenizer.GetIdentifier);

            // skip identifier
            _tokenizer.NextToken();

            return n;
        }

        // is string ?
        if (_tokenizer.CurrentToken == Token.STR)
        {
            NodeString n = new(_tokenizer.GetString);

            // skip string
            _tokenizer.NextToken();

            return n;
        }

        // is bool ?
        if (_tokenizer.CurrentToken == Token.BOOL)
        {
            NodeBool n = new(_tokenizer.GetBool);

            // skip bool
            _tokenizer.NextToken();

            return n;
        }

        throw new ParserException(
            ParserExceptionType.INVALID_TOKEN,
            $"can't parse token:{_tokenizer.CurrentToken}");
    }
}