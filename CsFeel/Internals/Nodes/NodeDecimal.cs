namespace CsFeel.Internals.Nodes;

public class NodeDecimal(decimal value) : Node<decimal>
{
    private readonly decimal _value = value;
    public override decimal Eval() => _value;

}