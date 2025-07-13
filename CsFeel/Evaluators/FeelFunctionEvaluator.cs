using System.Globalization;

namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static object? EvalFunctionCall(string funcName, List<FeelExpression> args, Dictionary<string, object?> context)
    {
        return funcName switch
        {
            //___________ string fn
            "string"
                => Eval(args[0], context)?.ToString(),
            "substring"
                => FnSubString(args, context),
            "string length"
                => FnStringLength(args, context),
            "upper case"
                => FnUpperCase(args, context),
            "lower case"
                => FnLowerCase(args, context),
            "contains"
                => FnContains(args, context),

            //___________ number fn
            "number"
                => FnNumber(args, context),
            "floor"
                => FnFloor(args, context),
            "ceiling"
                => FnCeiling(args, context),
            "abs"
                => FnAbs(args, context),
            "modulo"
                => FnModulo(args, context),
            "odd"
                => FnOdd(args, context),
            "even"
                => FnEven(args, context),
            "sqrt"
                => FnSqrt(args, context),

            //___________ other fn
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
            _ =>
                throw new FeelParserException(FeelParserError.INVALID_FUNCTION_NAME)
        };
    }

    //___________ string fn
    private static string? FnSubString(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count < 2 || args.Count > 3)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        var str = Eval(args[0], context)?.ToString();
        if (str == null)
        {
            return null;
        }

        var start = Eval(args[1], context) is decimal val
            ? (int)val
            : throw new FeelParserException(FeelParserError.INVALID_TYPE);

        if (args.Count == 2)
        {
            return str[(
                start > 0
                    ? start <= str.Length
                        ? start - 1
                        : str.Length
                    : Math.Max(0, str.Length - Math.Abs(start == 0 ? -1 : start))
            )..];
        }
        else
        {
            var subLen = Eval(args[2], context) is decimal lenVal
                ? (int)lenVal : throw new FeelParserException(FeelParserError.INVALID_TYPE);

            return str.Substring(
                start > 0
                    ? start <= str.Length
                        ? start - 1
                        : str.Length
                    : Math.Max(0, str.Length - Math.Abs(start == 0 ? -1 : start))
                , Math.Min(str.Length - 1, subLen));
        }
    }

    private static decimal? FnStringLength(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        var str = Eval(args[0], context)?.ToString();
        if (str == null)
        {
            return null;
        }

        return str.Length;
    }

    private static string? FnUpperCase(
       List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        var str = Eval(args[0], context)?.ToString();
        if (str == null)
        {
            return null;
        }

        return str.ToUpper();
    }

    private static string? FnLowerCase(
       List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        var str = Eval(args[0], context)?.ToString();
        if (str == null)
        {
            return null;
        }

        return str.ToLower();
    }

    private static bool? FnContains(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 2)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is string str && Eval(args[1], context) is string val
            ? str.Contains(val) : null;
    }


    //___________ number fn
    private static decimal? FnNumber(
       List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return decimal.TryParse(Eval(args[0], context)?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var n)
            ? n : null;
    }

    private static decimal? FnFloor(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is decimal n ? Math.Floor(n) : null;
    }

    private static decimal? FnCeiling(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is decimal n ? Math.Ceiling(n) : null;
    }

    private static decimal? FnAbs(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is decimal n ? Math.Abs(n) : null;
    }

    private static decimal? FnModulo(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 2)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is decimal divdn && Eval(args[1], context) is decimal divsr ? divdn % divsr : null;
    }

    private static bool? FnOdd(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 2)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is decimal n ? n % 2 != 0 : null;
    }

    private static bool? FnEven(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 2)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is decimal n ? n % 2 == 0 : null;
    }

    private static decimal? FnSqrt(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 2)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is decimal n ? (decimal)Math.Sqrt((double)n) : null;
    }
}