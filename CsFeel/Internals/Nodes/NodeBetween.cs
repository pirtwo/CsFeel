namespace CsFeel.Internals.Nodes;

public class NodeBetween(INode lowerBound, INode upperBound) : INode
{
    public readonly INode LowerBound = lowerBound;
    public void Accept(INodeVisitor visitor) => visitor.Visit(this);
}