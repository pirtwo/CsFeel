namespace CsFeel.Internals;

public class Context : IContext
{
    protected readonly Dictionary<string, object?> _variables = [];

    public T? ResolveVariable<T>(string name)
    {
        if (_variables.TryGetValue(name, out var value))
        {
            return (T?)value;
        }
        else
        {
            throw new NullReferenceException($"{name}");
        }
    }

    public void SetVariable<T>(string name, T value)
    {
        if (_variables.ContainsKey(name))
        {
            _variables[name] = value;
        }
        else
        {
            _variables.Add(name, value);
        }
    }
}