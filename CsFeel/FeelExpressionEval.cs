using System.Runtime.CompilerServices;

namespace CsFeel;

public static class FeelExpressionEval
{
    public static object? Eval(
        FeelExpression expression, Dictionary<string, object> context)
    {
        return expression switch
        {
            FeelLiteral x => x.Value,
            FeelList x => x.Items.Select(expr => Eval(expr, context)).ToList(),
            FeelVariable x => context.TryGetValue(x.Name, out var value) ? value : null,
            FeelFunctionCall x => EvalFunctionCall(x.Name, [.. x.Args], context),
            FeelUnary x => EvalUnary(x.Op, x.Right, context),
            FeelBinary x => EvalBinary(x.Left, x.Operator, x.Right, context),
            _ => throw new Exception(""),
        };
    }

    private static object? EvalBinary(
        FeelExpression lhs, string op, FeelExpression rhs, Dictionary<string, object> context)
    {
        var lhsVal = Eval(lhs, context);
        var rhsVal = Eval(rhs, context);

        return (lhsVal, rhsVal) switch
        {
            var (l, r) when l is null && r is null => EvalBinaryOperatorNull(op),
            var (l, r) when l is bool lv && r is null => EvalBinaryOperatorNull(lv, op),
            var (l, r) when l is null && r is bool rv => EvalBinaryOperatorNull(rv, op),
            var (l, r) when l is string lv && r is null => EvalBinaryOperatorNull(lv, op),
            var (l, r) when l is null && r is string rv => EvalBinaryOperatorNull(rv, op),
            var (l, r) when l is bool lv && r is bool rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is bool lv && r is string rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is string lv && r is bool rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is string lv && r is string rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is decimal lv && r is decimal rv => EvalBinaryOperator(lv, rv, op),
            _ => throw new FeelParserException(FeelParserError.INVALID_OPERATION, op)
        };
    }

    private static object? EvalUnary(
        string op, FeelExpression lhs, Dictionary<string, object> context)
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

    private static object? EvalFunctionCall(
        string funcName, List<FeelExpression> args, Dictionary<string, object> context)
    {
        return funcName switch
        {
            "not" =>
                Eval(args[0], context) is bool v
                ? !v
                : throw new FeelParserException(FeelParserError.INVALID_ARGUMENT),
            "date" =>
                DateTime.TryParse(Eval(args[0], context) as string, out var date)
                ? date.Date
                : throw new FeelParserException(FeelParserError.INVALID_ARGUMENT, date.ToString()),
            "time" =>
                TimeSpan.TryParse(Eval(args[0], context) as string, out var time)
                ? time
                : throw new FeelParserException(FeelParserError.INVALID_ARGUMENT, time.ToString()),
            "date and time" =>
                DateTime.TryParse(Eval(args[0], context) as string, out var dateTime)
                ? dateTime
                : throw new FeelParserException(FeelParserError.INVALID_ARGUMENT, dateTime.ToString()),
            _ =>
                throw new FeelParserException(FeelParserError.INVALID_FUNCTION_NAME)
        };
    }

    private static object? EvalBinaryOperator(decimal l, decimal r, string op) => op switch
    {
        "+" => l + r,
        "-" => l - r,
        "*" => l * r,
        "/" => l / r,
        ">" => l > r,
        "<" => l < r,
        "=" => l == r,
        ">=" => l >= r,
        "<=" => l <= r,
        "!=" => l != r,
        "or" => null,
        "and" => null,
        _ => throw new FeelParserException(FeelParserError.INVALID_OPERATION, op)
    };
    private static object? EvalBinaryOperator(string l, string r, string op) => op switch
    {
        "+" => l + r,
        "-" => null,
        "*" => null,
        "/" => null,
        ">" => l.CompareTo(r) > 0,
        "<" => l.CompareTo(r) < 0,
        "=" => l.CompareTo(r) == 0,
        ">=" => l.CompareTo(r) >= 0,
        "<=" => l.CompareTo(r) <= 0,
        "!=" => l.CompareTo(r) != 0,
        "or" => null,
        "and" => null,
        _ => throw new FeelParserException(FeelParserError.INVALID_OPERATION, op)
    };
    private static object? EvalBinaryOperator(string l, bool r, string op) => op switch
    {
        "+" => null,
        "-" => null,
        "*" => null,
        "/" => null,
        "=" => false,
        "!=" => true,
        "or" => r ? true : null,
        "and" => r ? null : false,
        _ => throw new FeelParserException(FeelParserError.INVALID_OPERATION, op)
    };
    private static object? EvalBinaryOperator(bool l, bool r, string op) => op switch
    {
        "+" => null,
        "-" => null,
        "*" => null,
        "/" => null,
        "=" => l == r,
        "!=" => l != r,
        "or" => l || r,
        "and" => l && r,
        _ => throw new FeelParserException(FeelParserError.INVALID_OPERATION, op)
    };
    private static object? EvalBinaryOperator(bool l, string r, string op) => op switch
    {
        "+" => null,
        "-" => null,
        "*" => null,
        "/" => null,
        "=" => false,
        "!=" => true,
        "or" => l ? true : null,
        "and" => l ? null : false,
        _ => throw new FeelParserException(FeelParserError.INVALID_OPERATION, op)
    };
    private static object? EvalBinaryOperatorNull(string op) => op switch
    {
        "+" => null,
        "-" => null,
        "*" => null,
        "/" => null,
        "=" => true,
        "!=" => false,
        _ => throw new FeelParserException(FeelParserError.INVALID_OPERATION, op)
    };
    private static object? EvalBinaryOperatorNull(bool operand, string op) => op switch
    {
        "+" => null,
        "-" => null,
        "*" => null,
        "/" => null,
        "=" => false,
        "!=" => true,
        "or" => operand ? true : null,
        "and" => operand ? null : false,
        _ => throw new FeelParserException(FeelParserError.INVALID_OPERATION, op)
    };
    private static object? EvalBinaryOperatorNull(string _, string op) => op switch
    {
        "+" => null,
        "-" => null,
        "*" => null,
        "/" => null,
        "=" => false,
        "!=" => true,
        "or" => null,
        "and" => null,
        _ => throw new FeelParserException(FeelParserError.INVALID_OPERATION, op)
    };
}