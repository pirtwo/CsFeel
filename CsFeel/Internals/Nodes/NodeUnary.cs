namespace CsFeel.Internals.Nodes;

public class NodeUnary(INode<decimal> rhs, Func<decimal, decimal> op) : INode<decimal>
{
    private readonly INode<decimal> _rhs = rhs;
    private readonly Func<decimal, decimal> _op = op;
    public decimal Eval() => _op(_rhs.Eval());
}