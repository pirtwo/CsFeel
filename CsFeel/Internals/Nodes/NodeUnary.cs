namespace CsFeel.Internals.Nodes;

public class NodeUnary(
    Node<decimal> rhs,
    Func<decimal, decimal> op) : Node<decimal>
{
    private readonly Node<decimal> _rhs = rhs;
    private readonly Func<decimal, decimal> _op = op;
    public override decimal Eval() => _op(_rhs.Eval());
}