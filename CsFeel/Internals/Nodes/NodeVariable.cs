namespace CsFeel.Internals.Nodes;

public class NodeVariable : INode<object?>
{
    protected IContext _context;
    protected string _name;

    public NodeVariable(IContext context, string name, object? value = null)
    {
        _context = context;
        _name = name;
        _context.SetVariable(name, value);
    }

    public object? Eval() => _context.ResolveVariable<object?>(_name);
}