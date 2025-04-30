namespace CsFeel.Internals.Nodes;

public class NodeNumeric(
    Node<decimal> lhs,
    Node<decimal> rhs,
    Func<decimal, decimal, decimal> op) : Node<decimal>
{
    private readonly Node<decimal> _lhs = lhs;
    private readonly Node<decimal> _rhs = rhs;
    private readonly Func<decimal, decimal, decimal> _op = op;
    public override decimal Eval() => _op(_lhs.Eval(), _rhs.Eval());
}