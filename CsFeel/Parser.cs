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
        var lhs = ParseCompare();

        while (true)
        {
            if (_tokenizer.CurrentToken == Token.POW)
            {
                var op = _tokenizer.CurrentToken ?? throw new Exception("");

                // skip **
                _tokenizer.NextToken();

                var rhs = ParseCompare();

                lhs = new NodeArithmetic(lhs, rhs, op);
            }
            else
            {
                return lhs;
            }
        }
    }

    protected INode ParseCompare()
    {
        var lhs = ParseUnary();

        var op = _tokenizer.CurrentToken ?? throw new Exception("");

        if (op == Token.CMP_GT
            || op == Token.CMP_GTE
            || op == Token.CMP_LT
            || op == Token.CMP_LTE
            || op == Token.CMP_EQ
            || op == Token.CMP_NEQ
            || op == Token.CMP_BTW)
        {
            // skip
            _tokenizer.NextToken();

            var rhs = ParseUnary();

            return new NodeCompare(lhs, rhs, op);
        }
        else
        {
            return lhs;
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
            decimal value = _tokenizer.GetNumber;

            _tokenizer.NextToken();

            return new NodeNumber(value);
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
                    ParserExceptionType.SYNTAX_ERROR, $"invalid char, expected: ')'");
            }

            // skip ')'
            _tokenizer.NextToken();

            return n;
        }

        // is bracket
        if (_tokenizer.CurrentToken == Token.BRK_OPE)
        {
            // skip '['
            _tokenizer.NextToken();

            List<INode> list = [];
            while (true)
            {
                var n = ParseAddSub();

                list.Add(n);

                if (_tokenizer.CurrentToken == Token.BRK_CLO)
                {
                    // skip ']'
                    _tokenizer.NextToken();
                    break;
                }

                if (_tokenizer.CurrentToken == Token.COMMA)
                {
                    // skip ','
                    _tokenizer.NextToken();
                    continue;
                }
                else
                {
                    throw new ParserException(ParserExceptionType.SYNTAX_ERROR, $"invalid char, expected: ',' or ']'");
                }
            }

            return new NodeList(list);
        }

        // is string ?
        if (_tokenizer.CurrentToken == Token.STR)
        {
            NodeString n = new(_tokenizer.GetString);

            // skip string
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

        // is keyword ?
        if (_tokenizer.CurrentToken == Token.KWD)
        {
            if (_tokenizer.GetKeyword == Keyword.NULL)
            {
                // skip identifier
                _tokenizer.NextToken();

                return new NodeNull();
            }
            if (_tokenizer.GetKeyword == Keyword.TRUE || _tokenizer.GetKeyword == Keyword.FALSE)
            {
                var n = new NodeBool(_tokenizer.GetKeyword == Keyword.TRUE);

                // skip identifier
                _tokenizer.NextToken();

                return n;
            }
            if (_tokenizer.GetKeyword == Keyword.AND)
            {
                var n = new NodeBool(_tokenizer.GetKeyword == Keyword.TRUE);

                // skip identifier
                _tokenizer.NextToken();

                return n;
            }

        }


        throw new ParserException(
            ParserExceptionType.INVALID_TOKEN,
            $"can't parse token:{_tokenizer.CurrentToken}");
    }
}