namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{

    private static bool EvalIn(
        FeelExpression valueExpr,
        FeelExpression collectionExpr,
        Dictionary<string, object?> context)
    {
        var lhsValue = Eval(valueExpr, context);
        var rhsValue = Eval(collectionExpr, context);

        if (rhsValue is IEnumerable<object?> seq)
            return seq.Any(item => Equals(item, lhsValue));

        if (rhsValue is IDictionary<string, object?> dict && lhsValue is string key)
            return dict.ContainsKey(key);

        throw new FeelParserException(FeelParserError.INVALID_OPERATION);
    }
}