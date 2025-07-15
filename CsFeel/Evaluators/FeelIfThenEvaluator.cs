namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static object? EvalIfThen(
        FeelExpression conditionExpr, FeelExpression thenExpr, FeelExpression elseExpr, Dictionary<string, object?> context) =>
            Eval(conditionExpr, context) is bool condition
            ? condition
                ? Eval(thenExpr, context)
                : Eval(elseExpr, context)
            : throw new FeelParserException(FeelParserError.IFTHEN_INVALID_CONDITION_VALUE);
}