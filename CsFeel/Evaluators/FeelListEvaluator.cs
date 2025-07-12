namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static object? EvalList(IEnumerable<FeelExpression> items, Dictionary<string, object> context) =>
        items.Select(expr => Eval(expr, context)).ToList();
}