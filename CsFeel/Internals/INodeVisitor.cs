using CsFeel.Internals.Nodes;

namespace CsFeel.Internals;

public interface INodeVisitor
{
    DataType GetResultType();
    void SetResultType(DataType type);

    object? GetResult();
    void SetResult(object? result);

    void Visit(NodeArithmetic node);
    void Visit(NodeUnary node);
    void Visit(NodeNumber node);
    void Visit(NodeString node);
    void Visit(NodeBool node);
    void Visit(NodeVariable node);
    void Visit(NodeNull node);
    void Visit(NodeBetween node);
    void Visit(NodeCompare node);
}