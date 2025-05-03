namespace CsFeel.Internals.Nodes;

public interface ITestNode
{
    T Eval<T>();
}

public class TestNode : ITestNode
{
    decimal _value;

    public T Eval<T>()
    {
        return (T)Convert.ChangeType(_value, typeof(T));
    }
}

public class TestNode2
{
    TestNode _t;

    public TestNode2()
    {
        var v = _t.Eval<decimal>();
    }
}