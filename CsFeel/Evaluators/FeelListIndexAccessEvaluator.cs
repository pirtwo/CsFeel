namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static object? EvalListIndexAccess(
        FeelExpression ListExpr, FeelExpression IndexExpr, Dictionary<string, object?> context)
    {
        var listExprValue = Eval(ListExpr, context);
        var indexExprValue = Eval(IndexExpr, context);

        return listExprValue is List<object?> list
            && indexExprValue is decimal index
            && index > 0
            && index <= list.Count
                ? list[(int)index - 1]
                : null;
    }
}