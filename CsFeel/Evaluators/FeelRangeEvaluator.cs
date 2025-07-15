namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
{
    private static List<object?> EvalRange(
        FeelExpression lowerBound,
        FeelExpression upperBound,
        bool inclusiveLower,
        bool inclusiveUpper,
        Dictionary<string, object?> context)
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

        throw new FeelParserException(
            FeelParserError.RANGE_INVALID_TYPE,
            $"{lowerBoundValue?.GetType().Name},{upperBoundValue?.GetType().Name}");
    }
}