namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static List<object?> EvalFor(
        string variable,
        FeelExpression collection,
        FeelExpression body,
        Dictionary<string, object?> context)
    {
        var list = Eval(collection, context);

        if (list is IEnumerable<object?> items)
        {
            var result = new List<object?>();

            foreach (var item in items)
            {
                var localCtx = new Dictionary<string, object?>(context)
                {
                    [variable] = item
                };
                result.Add(Eval(body, localCtx));
            }

            return result;
        }

        throw new FeelParserException(FeelParserError.INVALID_TYPE);
    }
}