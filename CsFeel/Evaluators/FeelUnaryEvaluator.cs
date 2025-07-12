namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static object? EvalUnary(string op, FeelExpression lhs, Dictionary<string, object> context)
    {
        var rhsVal = Eval(lhs, context);
        if (rhsVal is not decimal)
        {
            throw new FeelParserException(FeelParserError.INVALID_RIGHT_HAND_VALUE);
        }

        return op switch
        {
            "+" => rhsVal,
            "-" => -(decimal)rhsVal,
            _ => throw new FeelParserException(FeelParserError.INVALID_OPERATION, op),
        };
    }
}