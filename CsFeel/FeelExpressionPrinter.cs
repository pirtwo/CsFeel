namespace CsFeel;

public static class FeelExpressionPrinter
{
    public static void Print(FeelExpression expr, string indent = "", bool isLast = true)
    {

        var marker = isLast ? "└─ " : "├─ ";
        Console.WriteLine(indent + marker + NodeLabel(expr));


        indent += isLast ? "   " : "│  ";


        var children = GetChildren(expr).ToList();
        for (int i = 0; i < children.Count; i++)
        {
            var (label, childExpr) = children[i];

            if (label != null)
                Console.WriteLine(indent + (i == children.Count - 1 ? "└─ " : "├─ ") + $"{label}:");
            var nextIndent = indent + (label != null
                                       ? (i == children.Count - 1 ? "   " : "│  ")
                                       : "");
            Print(childExpr, nextIndent, i == children.Count - 1);
        }
    }


    private static string NodeLabel(FeelExpression expr) => expr switch
    {
        FeelLiteral lit => $"Literal({lit.Value})",
        FeelVariable v => $"Variable({v.Name})",
        FeelContext ctx => $"Context[{ctx.Properties.Count}] entries",
        FeelContextPropertyAccess pa => $"PropertyAccess(.{pa.PropertyName})",
        FeelFunctionCall fc => $"FunctionCall({fc.Name} args={fc.Args.Count()})",
        FeelUnary u => $"Unary({u.Op})",
        FeelBinary b => $"Binary({b.Operator})",
        FeelIfElse _ => "IfExpression",
        FeelSome _ => "SomeExpression",
        FeelBetween _ => "BetweenExpression",
        FeelInstanceOf io => $"InstanceOf({io.TypeName})",
        _ => expr.GetType().Name
    };


    private static IEnumerable<(string? slot, FeelExpression child)> GetChildren(FeelExpression expr)
    {
        switch (expr)
        {
            case FeelUnary u:
                yield return (null, u.Right);
                break;

            case FeelBinary b:
                yield return ("Left", b.Left);
                yield return ("Right", b.Right);
                break;

            case FeelFunctionCall fc:
                for (int i = 0; i < fc.Args.Count(); i++)
                    yield return ($"Arg[{i}]", fc.Args.ToList()[i]);
                break;

            case FeelContextPropertyAccess pa:
                yield return ("Target", pa.Target);
                break;

            case FeelContext ctx:
                foreach (var kv in ctx.Properties)
                    yield return ($"Entry[{kv.Key}]", kv.Value);
                break;

            case FeelIfElse ite:
                yield return ("Condition", ite.Condition);
                yield return ("ThenBranch", ite.ThenExpr);
                yield return ("ElseBranch", ite.ElseExpr);
                break;

            // case FeelSome se:
            //     yield return ("Collection", se.Predicate);
            //     yield return ("Predicate", se.Predicate);
            //     break;

            case FeelBetween be:
                yield return ("Value", be.Left);
                yield return ("Lower", be.LowerBoundExpr);
                yield return ("Upper", be.UpperBoundExpr);
                break;

            case FeelInstanceOf io:
                yield return ("Value", io.Left);
                break;

            default:
                yield break;
        }
    }
}