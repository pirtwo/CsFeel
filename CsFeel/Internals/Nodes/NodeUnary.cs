namespace CsFeel.Internals.Nodes;

public class NodeUnary(INode rhs, Token operation) : INode
{
    public readonly INode Rhs = rhs;
    public readonly Token Operation = operation;
    public void Accept(INodeVisitor visitor) => visitor.Visit(this);
}