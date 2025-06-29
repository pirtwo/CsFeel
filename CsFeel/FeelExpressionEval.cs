namespace CsFeel;

public static class FeelExpressionEval
{
    public static object? Eval(FeelExpression expression, Dictionary<string, object> context)
    {
        return expression switch
        {
            FeelLiteral x => x.Value,
            FeelVariable x => context.TryGetValue(x.Name, out var value) ? value : null,
            FeelArithmetic x => EvalArithmetic(x, context),
            _ => throw new Exception(""),
        };
    }

    private static decimal EvalArithmetic(FeelArithmetic x, Dictionary<string, object> context)
    {
        var lhsVal = Convert.ToDecimal(Eval(x.Left, context));
        var rhsVal = Convert.ToDecimal(Eval(x.Right, context));

        return x.Op switch
        {
            "+" => lhsVal + rhsVal,
            "-" => lhsVal - rhsVal,
            "*" => lhsVal * rhsVal,
            "/" => lhsVal / rhsVal,
            _ => throw new Exception(""),
        };
    }
}