namespace CsFeel;

public static class FeelExpressionEval
{
    public static object? Eval(FeelExpression expression, Dictionary<string, object> context) => expression switch
    {
        FeelLiteral
            x => x.Value,

        FeelUnary
            x => EvalUnary(x.Op, x.Right, context),

        FeelBinary
            x => EvalBinary(x.Left, x.Operator, x.Right, context),

        FeelBetween
            x => EvalBetween(x.Left, x.LowerBoundExpr, x.UpperBoundExpr, context),

        FeelVariable
            x => context.TryGetValue(x.Name, out var value) ? value : null,

        FeelInstanceOf
            x => EvalInstanceOf(x.Left, x.TypeName, context),

        FeelFunctionCall
            x => EvalFunctionCall(x.Name, [.. x.Args], context),

        FeelList
            x => x.Items.Select(expr => Eval(expr, context)).ToList(),

        FeelRange
            x => EvalRange(x.LowerBound, x.UpperBound, x.InclusiveLower, x.InclusiveUpper, context),

        FeelSome
            x => EvalSome(x.Variable, x.Collection, x.Predicate, context),

        FeelIn
            x => EvalIn(x.ValueExpr, x.CollectionExpr, context),

        FeelContext
            x => x.Properties.ToDictionary(p => p.Key, p => Eval(p.Value, context)),

        FeelContextPropertyAccess
            x => EvalContextPropertyAccess(x.Target, x.PropertyName, context),

        _ => throw new Exception(""),
    };

    private static bool EvalIn(
        FeelExpression valueExpr,
        FeelExpression collectionExpr,
        Dictionary<string, object> context)
    {
        var lhsValue = Eval(valueExpr, context);
        var rhsValue = Eval(collectionExpr, context);

        if (rhsValue is IEnumerable<object?> seq)
            return seq.Any(item => Equals(item, lhsValue));

        if (rhsValue is IDictionary<string, object?> dict && lhsValue is string key)
            return dict.ContainsKey(key);

        throw new FeelParserException(FeelParserError.INVALID_OPERATION);
    }

    private static bool EvalSome(
        string variable,
        FeelExpression collection,
        FeelExpression predicate,
        Dictionary<string, object> context)
    {
        var items = Eval(collection, context) as IEnumerable<object>
            ?? throw new FeelParserException(FeelParserError.INVALID_OPERATION);
        foreach (var item in items)
        {
            var ctx2 = new Dictionary<string, object>(context)
            {
                [variable] = item
            };

            var val = Eval(predicate, ctx2);
            if (val is bool bv)
            {
                if (bv)
                {
                    return true;
                }
            }
            else
            {
                throw new FeelParserException(FeelParserError.INVALID_OPERATION);
            }
        }

        return false;
    }

    private static List<object?> EvalRange(
        FeelExpression lowerBound,
        FeelExpression upperBound,
        bool inclusiveLower,
        bool inclusiveUpper,
        Dictionary<string, object> context)
    {
        var lowerBoundValue = Eval(lowerBound, context);
        var upperBoundValue = Eval(upperBound, context);

        if (lowerBoundValue is decimal lvDecimal && upperBoundValue is decimal uvDecimal)
        {
            var stepLow = inclusiveLower ? lvDecimal : lvDecimal + 1;
            var stepHigh = inclusiveUpper ? uvDecimal : uvDecimal - 1;
            var list = new List<object?>();
            for (var v = stepLow; v <= stepHigh; v += 1m)
            {
                list.Add(v);
            }
            return list;
        }

        if (lowerBoundValue is DateTime lvDate && upperBoundValue is DateTime uvDate)
        {
            var current = inclusiveLower ? lvDate : lvDate.AddDays(1);
            var end = inclusiveUpper ? uvDate : uvDate.AddDays(-1);
            var list = new List<object?>();
            for (var d = current; d <= end; d = d.AddDays(1))
            {
                list.Add(d);
            }
            return list;
        }

        throw new FeelParserException(FeelParserError.UNSUPPORTED_RANGE_TYPE);
    }

    private static object? EvalContextPropertyAccess(
        FeelExpression target,
        string propertyName,
        Dictionary<string, object> context)
    {
        var targetValue = Eval(target, context);
        if (targetValue is Dictionary<string, object?> tv)
        {
            return tv.TryGetValue(propertyName, out var propValue)
                ? propValue
                : null;
        }
        else
        {
            throw new FeelParserException(FeelParserError.INVALID_PROPERTY_ACCESS);
        }
    }

    private static bool EvalInstanceOf(
        FeelExpression left,
        string typeName,
        Dictionary<string, object> context) => typeName switch
        {
            "any" => Eval(left, context) is not null,
            "time" => Eval(left, context) is TimeSpan,
            "date" => Eval(left, context) is DateTime,
            "list" => Eval(left, context) is List<object?>,
            "number" => Eval(left, context) is decimal,
            "string" => Eval(left, context) is string,
            "boolean" => Eval(left, context) is bool,
            _ => throw new FeelParserException(FeelParserError.INVALID_OPERAND)
        };

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

    private static object? EvalUnary(
        string op,
        FeelExpression lhs,
        Dictionary<string, object> context)
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
        string funcName,
        List<FeelExpression> args,
        Dictionary<string, object> context)
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
            "string" =>
                Eval(args[0], context)?.ToString(),
            _ =>
                throw new FeelParserException(FeelParserError.INVALID_FUNCTION_NAME)
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
            var (l, r) when l is null && r is bool rv => EvalBinaryOperatorNull(rv, op),
            var (l, r) when l is null && r is string rv => EvalBinaryOperatorNull(rv, op),
            var (l, r) when l is bool lv && r is null => EvalBinaryOperatorNull(lv, op),
            var (l, r) when l is bool lv && r is bool rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is bool lv && r is string rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is string lv && r is null => EvalBinaryOperatorNull(lv, op),
            var (l, r) when l is string lv && r is bool rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is string lv && r is string rv => EvalBinaryOperator(lv, rv, op),
            var (l, r) when l is decimal lv && r is decimal rv => EvalBinaryOperator(lv, rv, op),
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
}