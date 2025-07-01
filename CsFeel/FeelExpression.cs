namespace CsFeel;

public abstract record FeelExpression { }

public record FeelLiteral(object? Value) : FeelExpression;
public record FeelVariable(string Name) : FeelExpression;
public record FeelUnary(string Op, FeelExpression Right) : FeelExpression;
public record FeelBinary(FeelExpression Left, string Operator, FeelExpression Right) : FeelExpression;
public record FeelIfElse(FeelExpression Condition, FeelExpression ThenExpr, FeelExpression ElseExpr) : FeelExpression;
public record FeelList(IEnumerable<FeelExpression> Items) : FeelExpression;
public record FeelRange(FeelExpression Start, FeelExpression End) : FeelExpression;
public record FeelSome(string Variable, FeelExpression InExpr, FeelExpression Predicate) : FeelExpression;
public record FeelFunctionCall(string Name, IEnumerable<FeelExpression> Args) : FeelExpression;
