namespace CsFeel.Internals.Nodes;

public class NodeBool(bool value) : INode
{
    public bool Value { get; set; } = value;
    public void Accept(INodeVisitor visitor) => visitor.Visit(this);
}