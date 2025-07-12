namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static bool EvalInstanceOf(
        FeelExpression left,
        string typeName,
        Dictionary<string, object?> context) => typeName switch
        {
            "any" => Eval(left, context) is not null,
            "time" => Eval(left, context) is TimeSpan,
            "date" => Eval(left, context) is DateTime,
            "list" => Eval(left, context) is List<object?>,
            "number" => Eval(left, context) is decimal,
            "string" => Eval(left, context) is string,
            "boolean" => Eval(left, context) is bool,
            "context" => Eval(left, context) is Dictionary<string, FeelExpression>,
            "function" => Eval(left, context) is Dictionary<string, FeelExpression>,
            _ => throw new FeelParserException(FeelParserError.INVALID_OPERAND)
        };
}