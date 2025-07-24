namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static object? EvalBinary(
        FeelExpression lhs, string op, FeelExpression rhs, Dictionary<string, object?> context)
    {
        var lhsVal = Eval(lhs, context);
        var rhsVal = Eval(rhs, context);

        return (lhsVal, rhsVal) switch
        {
            var (l, r) when l is null && r is null => EvalBinaryOperatorNull(op),
            var (l, r) when l is null && r is bool rv => EvalBinaryOperatorNull(rv, op),
            var (l, r) when l is null && r is string rv => EvalBinaryOperatorNull(rv, op),
            var (l, r) when l is null && r is decimal rv => EvalBinaryOperatorNull(rv, op),
            var (l, r) when l is bool lv && r is null => EvalBinaryOperatorNull(lv, op),
            var (l, r) when l is bool lv && r is bool rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is bool lv && r is string rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is string lv && r is null => EvalBinaryOperatorNull(lv, op),
            var (l, r) when l is string lv && r is bool rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is string lv && r is string rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is decimal lv && r is decimal rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is DateTime lv && r is DateTime rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is List<object?> lv && r is List<object?> rv => EvalBinaryOperator(lv, rv, op),
            _ => throw new FeelParserException(FeelParserError.INVALID_OPERATION, op)
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
        "**" => (decimal)Math.Pow((double)l, (double)r),
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

    private static object? EvalBinaryOperatorNull(decimal operand, string op) => op switch
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

    private static object? EvalBinaryOperator(DateTime l, DateTime r, string op) => op switch
    {
        "+" => null,
        "-" => null,
        "*" => null,
        "/" => null,
        ">" => l > r,
        "<" => l < r,
        "=" => l == r,
        ">=" => l >= r,
        "<=" => l <= r,
        "!=" => l != r,
        "**" => null,
        "or" => null,
        "and" => null,
        _ => throw new FeelParserException(FeelParserError.INVALID_OPERATION, op)
    };

    private static object? EvalBinaryOperator(List<object?> l, List<object?> r, string op) => op switch
    {
        "+" => null,
        "-" => null,
        "*" => null,
        "/" => null,
        ">" => null,
        "<" => null,
        "=" => Helper.IsEqual(l, r),
        ">=" => null,
        "<=" => null,
        "!=" => !Helper.IsEqual(l, r),
        "**" => null,
        "or" => null,
        "and" => null,
        _ => throw new FeelParserException(FeelParserError.INVALID_OPERATION, op)
    };
}