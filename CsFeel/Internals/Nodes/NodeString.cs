namespace CsFeel.Internals.Nodes;

public class NodeString(string value) : INode
{
    public string Value { get; set; } = value;
    public void Accept(INodeVisitor visitor) => visitor.Visit(this);
}