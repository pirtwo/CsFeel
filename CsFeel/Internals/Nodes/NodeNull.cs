namespace CsFeel.Internals.Nodes;

public class NodeNull() : INode
{
    public void Accept(INodeVisitor visitor) => visitor.Visit(this);
}