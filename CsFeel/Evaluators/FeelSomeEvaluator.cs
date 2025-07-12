namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static bool EvalSome(
        string variable,
        FeelExpression collection,
        FeelExpression predicate,
        Dictionary<string, object> context)
    {
        var items = Eval(collection, context) as IEnumerable<object>
            ?? throw new FeelParserException(FeelParserError.INVALID_OPERATION);
        foreach (var item in items)
        {
            var ctx2 = new Dictionary<string, object>(context)
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
                throw new FeelParserException(FeelParserError.INVALID_OPERATION);
            }
        }

        return false;
    }

}