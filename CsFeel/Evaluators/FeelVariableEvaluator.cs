namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static object? EvalVariable(string name, Dictionary<string, object?> context)
    {
        return context.TryGetValue(name, out var value) ? value : null;
    }
}