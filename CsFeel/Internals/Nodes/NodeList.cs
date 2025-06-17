namespace CsFeel.Internals.Nodes;

public class NodeList(List<INode> list) : INode
{
    public List<INode> Value { get; set; } = list;
    public void Accept(INodeVisitor visitor) => visitor.Visit(this);
}