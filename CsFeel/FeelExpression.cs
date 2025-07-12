namespace CsFeel;

public abstract record FeelExpression { }

public record FeelLiteral(object? Value) : FeelExpression;
public record FeelVariable(string Name) : FeelExpression;
public record FeelUnary(string Op, FeelExpression Right) : FeelExpression;
public record FeelBinary(FeelExpression Left, string Operator, FeelExpression Right) : FeelExpression;
public record FeelIfElse(FeelExpression Condition, FeelExpression ThenExpr, FeelExpression ElseExpr) : FeelExpression;
public record FeelBetween(FeelExpression Left, FeelExpression LowerBoundExpr, FeelExpression UpperBoundExpr) : FeelExpression;
public record FeelInstanceOf(FeelExpression Left, string TypeName) : FeelExpression;
public record FeelContext(Dictionary<string, FeelExpression> Properties) : FeelExpression;
public record FeelContextPropertyAccess(FeelExpression Target, string PropertyName) : FeelExpression;
public record FeelList(IEnumerable<FeelExpression> Items) : FeelExpression;
public record FeelRange(FeelExpression LowerBound, FeelExpression UpperBound, bool InclusiveLower, bool InclusiveUpper) : FeelExpression;
public record FeelSome(string Variable, FeelExpression Collection, FeelExpression Predicate) : FeelExpression;
public record FeelIn(FeelExpression ValueExpr, FeelExpression CollectionExpr) : FeelExpression;
public record FeelFunctionCall(string Name, IEnumerable<FeelExpression> Args) : FeelExpression;
public record FeelFor(string Variable, FeelExpression Collection, FeelExpression Body) : FeelExpression;
