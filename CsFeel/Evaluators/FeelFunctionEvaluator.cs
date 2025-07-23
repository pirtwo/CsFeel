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
            "replace"
                => FnReplace(args, context),

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


            //___________ bool fn
            "not" => FnNot(args, context),


            //___________ list fn
            "list contains"
                => FnListContains(args, context),
            "list replace"
                => FnListReplace(args, context),
            "count"
                => FnCount(args, context),
            "min"
                => FnMin(args, context),
            "max"
                => FnMax(args, context),
            "sum"
                => FnSum(args, context),
            "median"
                => FnMedian(args, context),
            "all"
                => FnAll(args, context),
            "any"
                => FnAny(args, context),
            "append"
                => FnAppend(args, context),
            "reverse"
                => FnReverse(args, context),
            "index of"
                => FnIndexOf(args, context),

            //___________ date fn
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

    private static string? FnReplace(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 3)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is string input
            && Eval(args[1], context) is string pattern
            && Eval(args[2], context) is string replace
            ? input.Replace(pattern, replace, StringComparison.InvariantCulture)
            : null;
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

        return Eval(args[0], context) is decimal divdn && Eval(args[1], context) is decimal divsr
            ? divdn - divsr * Math.Floor(divdn / divsr)
            : null;
    }

    private static bool? FnOdd(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is decimal n ? n % 2 != 0 : null;
    }

    private static bool? FnEven(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is decimal n ? n % 2 == 0 : null;
    }

    private static decimal? FnSqrt(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is decimal n ? (decimal)Math.Sqrt((double)n) : null;
    }


    //___________ bool fn
    private static bool? FnNot(
        List<FeelExpression> args, Dictionary<string, object?> context)
            => Eval(args[0], context) is bool v ? !v : null;


    //___________ list fn
    private static bool FnListContains(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 2)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is List<object?> list && list.Contains(Eval(args[1], context));
    }

    private static List<object?>? FnListReplace(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 3)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        if (Eval(args[0], context) is List<object?> list && Eval(args[1], context) is decimal index)
        {
            var i = (int)Math.Floor(index);

            if (index.Scale != 0 || i <= 0 || i > list.Count)
            {
                return null;
            }

            list[i - 1] = Eval(args[2], context);

            return list;
        }
        else
        {
            return null;
        }
    }

    private static decimal? FnCount(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is List<object?> list
            ? list.Count
            : null;
    }

    private static List<decimal>? FnIndexOf(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 2)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        var item = Eval(args[1], context);

        return Eval(args[0], context) is List<object?> list
            ? list.Aggregate(new
            {
                Index = 0,
                List = new List<decimal>()
            }, (acc, crr) => new
            {
                Index = acc.Index + 1,
                List = crr == item ? [.. acc.List, acc.Index] : acc.List
            },
            acc => acc.List) : null;
    }

    private static List<object?>? FnReverse(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        if (Eval(args[0], context) is List<object?> list)
        {
            list.Reverse();
            return list;
        }
        else
        {
            return null;
        }
    }

    private static List<object?>? FnAppend(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 2)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is List<object?> list
            ? [.. list.Append(Eval(args[1], context))]
            : null;
    }

    private static bool? FnAny(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        var expValue = Eval(args[0], context);

        return expValue is List<object?> list
            ? list.Any(x => x is bool v && v == true)
            : expValue is bool boolValue
                ? boolValue
                : null;
    }

    private static bool? FnAll(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        var expValue = Eval(args[0], context);

        return expValue is List<object?> list
            ? list.All(x => x is bool v && v == true) || list.Count == 0
            : expValue is bool boolValue
                ? boolValue == true
                : null;
    }

    private static decimal? FnMedian(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is List<object?> list && list.All(x => x is decimal) && list.Count > 0
            ? list.Select(x => (decimal)x!).Sum() / list.Count
            : null;
    }

    private static decimal? FnSum(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is List<object?> list && list.All(x => x is decimal) && list.Count > 0
            ? list.Select(x => (decimal)x!).Sum()
            : null;
    }

    private static decimal? FnMax(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is List<object?> list && list.All(x => x is decimal) && list.Count > 0
            ? list.Select(x => (decimal)x!).Max()
            : null;
    }

    private static decimal? FnMin(
        List<FeelExpression> args, Dictionary<string, object?> context)
    {
        if (args.Count != 1)
        {
            throw new FeelParserException(FeelParserError.INVALID_NUMBER_OF_ARGUMENTS);
        }

        return Eval(args[0], context) is List<object?> list && list.All(x => x is decimal) && list.Count > 0
            ? list.Select(x => (decimal)x!).Min()
            : null;
    }
}