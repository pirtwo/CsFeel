namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static bool EvalSome(
        string variable,
        FeelExpression collection,
        FeelExpression predicate,
        Dictionary<string, object?> context)
    {
        var items = Eval(collection, context);
        if (items is not IEnumerable<object>)
        {
            throw new FeelParserException(
                FeelParserError.SOME_INVALID_COLLECTION_VALUE, items?.ToString() ?? "");
        }

        foreach (var item in items as IEnumerable<object> ?? [])
        {
            var ctx2 = new Dictionary<string, object?>(context)
            {
                [variable] = item
            };

            var val = Eval(predicate, ctx2);
            if (val is bool bv)
            {
                if (bv)
                {
                    return true;
                }
            }
            else
            {
                throw new FeelParserException(
                    FeelParserError.SOME_INVALID_PREDICATE_VALUE, val?.ToString() ?? "");
            }
        }

        return false;
    }

}