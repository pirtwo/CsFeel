using CsFeel.Internals.Nodes;

namespace CsFeel.Internals;

public class Evaluator : INodeVisitor
{
    protected object? _result;
    protected DataType _resultType;

    public object? GetResult() => _result;
    public void SetResult(object? result) => _result = result;

    public DataType GetResultType() => _resultType;
    public void SetResultType(DataType type) => _resultType = type;

    public void Visit(NodeArithmetic node)
    {
        var lhsEval = new Evaluator();
        node.Lhs.Accept(lhsEval);

        var rhsEval = new Evaluator();
        node.Rhs.Accept(rhsEval);

        switch (node.Operation)
        {
            case Token.ADD:
                if (lhsEval.GetResultType() == DataType.STRING && rhsEval.GetResultType() == DataType.STRING)
                {
                    _result = (string?)lhsEval.GetResult() + (string?)rhsEval.GetResult();
                    _resultType = DataType.STRING;
                    break;
                }
                if (lhsEval.GetResultType() == DataType.NUMBER && rhsEval.GetResultType() == DataType.NUMBER)
                {
                    _result = (decimal?)lhsEval.GetResult() + (decimal?)rhsEval.GetResult();
                    _resultType = DataType.NUMBER;
                    break;
                }
                else
                {
                    _result = null;
                    _resultType = DataType.NULL;
                }

                break;
            case Token.SUB:
                if (lhsEval.GetResultType() == DataType.NUMBER && rhsEval.GetResultType() == DataType.NUMBER)
                {
                    _result = (decimal?)lhsEval.GetResult() - (decimal?)rhsEval.GetResult();
                    _resultType = DataType.NUMBER;
                    break;
                }
                else
                {
                    _result = null;
                    _resultType = DataType.NULL;
                }

                break;
            case Token.MUL:
                if (lhsEval.GetResultType() == DataType.NUMBER && rhsEval.GetResultType() == DataType.NUMBER)
                {
                    _result = (decimal?)lhsEval.GetResult() * (decimal?)rhsEval.GetResult();
                    _resultType = DataType.NUMBER;
                    break;
                }
                else
                {
                    _result = null;
                    _resultType = DataType.NULL;
                }

                break;
            case Token.DIV:
                if (lhsEval.GetResultType() == DataType.NUMBER && rhsEval.GetResultType() == DataType.NUMBER)
                {
                    _result = (decimal?)lhsEval.GetResult() / (decimal?)rhsEval.GetResult();
                    _resultType = DataType.NUMBER;
                    break;
                }
                else
                {
                    _result = null;
                    _resultType = DataType.NULL;
                }

                break;
            case Token.POW:
                if (lhsEval.GetResultType() == DataType.NUMBER && rhsEval.GetResultType() == DataType.NUMBER)
                {
                    var b = decimal.ToDouble((decimal?)lhsEval.GetResult() ?? throw new Exception());
                    var e = decimal.ToDouble((decimal?)rhsEval.GetResult() ?? throw new Exception());

                    _result = (decimal)Math.Pow(b, e);
                    _resultType = DataType.NUMBER;

                    break;
                }
                else
                {
                    _result = null;
                    _resultType = DataType.NULL;
                }

                break;
            default:
                break;
        }
    }

    public void Visit(NodeNumber node)
    {
        _result = node.Value;
        _resultType = DataType.NUMBER;
    }

    public void Visit(NodeVariable node)
    {
        throw new NotImplementedException();
    }

    public void Visit(NodeUnary node)
    {
        var rhsEval = new Evaluator();
        node.Rhs.Accept(rhsEval);

        switch (node.Operation)
        {
            case Token.ADD:
                _result = rhsEval._result;
                _resultType = rhsEval._resultType;
                break;
            case Token.SUB:
                _result = -(decimal?)rhsEval._result;
                _resultType = rhsEval._resultType;
                break;
            default:
                break;
        }
    }

    public void Visit(NodeString node)
    {
        _result = node.Value;
        _resultType = DataType.STRING;
    }

    public void Visit(NodeBool node)
    {
        _result = node.Value;
        _resultType = DataType.BOOLEAN;
    }

    public void Visit(NodeNull node)
    {
        _result = null;
        _resultType = DataType.NULL;
    }

    public void Visit(NodeBetween node)
    {
        throw new NotImplementedException();
    }

    public void Visit(NodeCompare node)
    {
        var lhsEval = new Evaluator();
        node.Lhs.Accept(lhsEval);

        var rhsEval = new Evaluator();
        node.Rhs.Accept(rhsEval);

        if (lhsEval.GetResultType() != rhsEval.GetResultType())
        {
            _result = null;
            _resultType = DataType.NULL;
            return;
        }

        _resultType = DataType.BOOLEAN;
        switch (node.Operation)
        {
            case Token.CMP_EQ:
                switch (lhsEval.GetResultType())
                {
                    case DataType.NULL:
                        _result = lhsEval.GetResult()! == rhsEval.GetResult()!;
                        break;
                    case DataType.BOOLEAN:
                        _result = (bool)lhsEval.GetResult()! == (bool)rhsEval.GetResult()!;
                        break;
                    case DataType.NUMBER:
                        _result = (decimal)lhsEval.GetResult()! == (decimal)rhsEval.GetResult()!;
                        break;
                    case DataType.STRING:
                        _result = (string)lhsEval.GetResult()! == (string)rhsEval.GetResult()!;
                        break;
                    default:
                        break;
                }
                break;
            case Token.CMP_NEQ:
                switch (lhsEval.GetResultType())
                {
                    case DataType.NULL:
                        _result = lhsEval.GetResult()! != rhsEval.GetResult()!;
                        break;
                    case DataType.BOOLEAN:
                        _result = (bool)lhsEval.GetResult()! != (bool)rhsEval.GetResult()!;
                        break;
                    case DataType.NUMBER:
                        _result = (decimal)lhsEval.GetResult()! != (decimal)rhsEval.GetResult()!;
                        break;
                    case DataType.STRING:
                        _result = (string)lhsEval.GetResult()! != (string)rhsEval.GetResult()!;
                        break;
                    default:
                        break;
                }
                break;
            case Token.CMP_LT:
                switch (lhsEval.GetResultType())
                {
                    case DataType.NULL:
                        _result = null;
                        break;
                    case DataType.NUMBER:
                        _result = (decimal)lhsEval.GetResult()! < (decimal)rhsEval.GetResult()!;
                        break;
                    case DataType.STRING:
                        _result = string.Compare((string)lhsEval.GetResult()!, (string)rhsEval.GetResult()!) < 0;
                        break;
                    default:
                        break;
                }
                break;
            case Token.CMP_LTE:
                switch (lhsEval.GetResultType())
                {
                    case DataType.NULL:
                        _result = null;
                        break;
                    case DataType.NUMBER:
                        _result = (decimal)lhsEval.GetResult()! <= (decimal)rhsEval.GetResult()!;
                        break;
                    case DataType.STRING:
                        _result = (string)lhsEval.GetResult()! == (string)rhsEval.GetResult()!
                            || string.Compare((string)lhsEval.GetResult()!, (string)rhsEval.GetResult()!) < 0;
                        break;
                    default:
                        break;
                }
                break;
            case Token.CMP_GT:
                switch (lhsEval.GetResultType())
                {
                    case DataType.NULL:
                        _result = null;
                        break;
                    case DataType.NUMBER:
                        _result = (decimal)lhsEval.GetResult()! > (decimal)rhsEval.GetResult()!;
                        break;
                    case DataType.STRING:
                        _result = string.Compare((string)lhsEval.GetResult()!, (string)rhsEval.GetResult()!) > 0;
                        break;
                    default:
                        break;
                }
                break;
            case Token.CMP_GTE:
                switch (lhsEval.GetResultType())
                {
                    case DataType.NULL:
                        _result = null;
                        break;
                    case DataType.NUMBER:
                        _result = (decimal)lhsEval.GetResult()! >= (decimal)rhsEval.GetResult()!;
                        break;
                    case DataType.STRING:
                        _result = (string)lhsEval.GetResult()! == (string)rhsEval.GetResult()!
                            || string.Compare((string)lhsEval.GetResult()!, (string)rhsEval.GetResult()!) > 0;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    public void Visit(NodeList node)
    {
        _result = node.Value;
        _resultType = DataType.LIST;
    }
}