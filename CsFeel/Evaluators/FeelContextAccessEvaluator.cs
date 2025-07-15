namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static object? EvalContextPropertyAccess(
        FeelExpression target,
        string propertyName,
        Dictionary<string, object?> context)
    {
        if (target is FeelContext ctx)
        {
            return ctx.Properties.TryGetValue(propertyName, out var expr)
                ? Eval(expr, context)
                : null;
        }

        var targetValue = Eval(target, context);
        if (targetValue is Dictionary<string, object?> tv)
        {
            return tv.TryGetValue(propertyName, out var propValue)
                ? propValue
                : null;
        }

        throw new FeelParserException(FeelParserError.CONTEXT_INVALID_TARGET_VALUE);
    }
}