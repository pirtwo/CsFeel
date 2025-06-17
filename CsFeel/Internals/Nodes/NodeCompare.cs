namespace CsFeel.Internals.Nodes;

public class NodeCompare(INode lhs, INode rhs, Token operation) : INode
{
    public readonly INode Lhs = lhs;
    public readonly INode Rhs = rhs;
    public readonly Token Operation = operation;
    public void Accept(INodeVisitor visitor) => visitor.Visit(this);
}