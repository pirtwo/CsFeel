using System.Globalization;

namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static object? EvalFunctionCall(
        string funcName,
        List<FeelExpression> args,
        Dictionary<string, object?> context)
    {
        return funcName switch
        {
            "string" =>
                Eval(args[0], context)?.ToString(),
            "not" =>
                Eval(args[0], context) is bool v
                ? !v
                : null,
            "date" =>
                DateTime.TryParse(Eval(args[0], context) as string, out var date)
                ? date.Date
                : null,
            "time" =>
                TimeSpan.TryParse(Eval(args[0], context) as string, out var time)
                ? time
                : null,
            "date and time" =>
                DateTime.TryParse(Eval(args[0], context) as string, out var dateTime)
                ? dateTime
                : null,
            "number" =>
                decimal.TryParse(Eval(args[0], context)?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
                    ? result
                    : null,
            _ =>
                throw new FeelParserException(FeelParserError.INVALID_FUNCTION_NAME)
        };
    }
}