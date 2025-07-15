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

        if (rhsValue is IDictionary<string, object?> dict)
        {
            if (lhsValue is string key)
            {
                return dict.ContainsKey(key);
            }
            else
            {
                throw new FeelParserException(
                    FeelParserError.IN_INVALID_LEFT_HAND_VALE, lhsValue?.ToString() ?? "");
            }
        }

        throw new FeelParserException(
            FeelParserError.IN_INVALID_RIGHT_HAND_VALE, rhsValue?.ToString() ?? "");
    }
}