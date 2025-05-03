namespace CsFeel.Internals.Nodes;

public class NodeNumber(decimal value) : INode<decimal>
{
    private readonly decimal _value = value;
    public decimal Eval() => _value;
}