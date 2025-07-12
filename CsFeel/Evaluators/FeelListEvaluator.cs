namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static List<object?> EvalList(IEnumerable<FeelExpression> items, Dictionary<string, object?> context)
    {
        return [.. items.Select(expr => Eval(expr, context))];
    }
}