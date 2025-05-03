namespace CsFeel.Internals.Nodes;

public class NodeArithmetic(
    INode<decimal> lhs,
    INode<decimal> rhs,
    Func<decimal, decimal, decimal> op) : INode<decimal>
{
    private readonly INode<decimal> _lhs = lhs;
    private readonly INode<decimal> _rhs = rhs;
    private readonly Func<decimal, decimal, decimal> _op = op;
    public decimal Eval() => _op(_lhs.Eval(), _rhs.Eval());
}