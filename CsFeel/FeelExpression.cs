namespace CsFeel;

public abstract record FeelExpression { }

public record FeelLiteral(object? Value) : FeelExpression;
public record FeelVariable(string Name) : FeelExpression;
public record FeelArithmetic(FeelExpression Left, string Op, FeelExpression Right) : FeelExpression;
public record FeelBoolComparison(FeelExpression Left, string Op, FeelExpression Right) : FeelExpression;
public record FeelIfElse(FeelExpression Condition, FeelExpression ThenExpr, FeelExpression ElseExpr) : FeelExpression;
public record FeelList(IEnumerable<FeelExpression> Items) : FeelExpression;
public record FeelRange(FeelExpression Start, FeelExpression End) : FeelExpression;
public record FeelSome(string Variable, FeelExpression InExpr, FeelExpression Predicate) : FeelExpression;