namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static Dictionary<string, object?> EvalContext(
        Dictionary<string,
        FeelExpression> props,
        Dictionary<string, object?> context)
            => props.ToDictionary(p => p.Key, p => Eval(p.Value, context));
}