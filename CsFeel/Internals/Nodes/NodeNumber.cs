namespace CsFeel.Internals.Nodes;

public class NodeNumber(decimal value) : INode
{
    public readonly decimal Value = value;
    public void Accept(INodeVisitor visitor) => visitor.Visit(this);
}