namespace CsFeel.Evaluators;

public static partial class FeelExpressionEvaluator
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
            x => EvalVariable(x.Name, context),

        FeelInstanceOf
            x => EvalInstanceOf(x.Left, x.TypeName, context),

        FeelFunctionCall
            x => EvalFunctionCall(x.Name, [.. x.Args], context),

        FeelList
            x => EvalList(x.Items, context),

        FeelRange
            x => EvalRange(x.LowerBound, x.UpperBound, x.InclusiveLower, x.InclusiveUpper, context),

        FeelSome
            x => EvalSome(x.Variable, x.Collection, x.Predicate, context),

        FeelIn
            x => EvalIn(x.ValueExpr, x.CollectionExpr, context),

        FeelContext
            x => EvalContext(x.Properties, context),

        FeelContextPropertyAccess
            x => EvalContextPropertyAccess(x.Target, x.PropertyName, context),

        _ => throw new FeelParserException(FeelParserError.INVALID_EXPRESSION),
    };
}