namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static object? EvalUnary(string op, FeelExpression lhs, Dictionary<string, object?> context)
    {
        var rhsVal = Eval(lhs, context);
        if (rhsVal is not decimal)
        {
            throw new FeelParserException(
                FeelParserError.UNARY_INVALID_RIGHT_HAND_VALUE, rhsVal?.ToString() ?? "");
        }

        return op switch
        {
            "+" => rhsVal,
            "-" => -(decimal)rhsVal,
            _ => throw new FeelParserException(FeelParserError.UNARY_INVALID_OPERATOR, op),
        };
    }
}