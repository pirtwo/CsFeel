namespace CsFeel.Internals.Nodes;

public interface INode
{
    void Accept(INodeVisitor visitor);
}