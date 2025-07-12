namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static bool EvalBetween(
        FeelExpression left,
        FeelExpression lowerBoundExpr,
        FeelExpression upperBoundExpr,
        Dictionary<string, object> context)
    {
        var leftVal = Eval(left, context);
        var lowerBoundVal = Eval(lowerBoundExpr, context);
        var upperBoundVal = Eval(upperBoundExpr, context);

        return (leftVal, lowerBoundVal, upperBoundVal) switch
        {
            var (x, y, z) when x is decimal v && y is decimal lb && z is decimal ub => v >= lb && v <= ub,
            var (x, y, z) when x is DateTime v && y is DateTime lb && z is DateTime ub => v >= lb && v <= ub,
            _ => throw new FeelParserException(FeelParserError.INVALID_OPERAND)
        };
    }
}