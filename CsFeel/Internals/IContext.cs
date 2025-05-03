namespace CsFeel.Internals;

public interface IContext
{
    void SetVariable<T>(string name, T value);
    T? ResolveVariable<T>(string name);
}