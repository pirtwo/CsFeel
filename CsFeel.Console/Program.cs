using Sprache;
using CsFeel;
using CsFeel.Evaluators;

Console.WriteLine("FEEL REPL - type 'exit' to quit");
var globals = new Dictionary<string, object?>();

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();
    if (line == null || line.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    if (string.IsNullOrWhiteSpace(line))
        continue;

    try
    {
        // 1) Parse
        var expr = FeelParser.Expr.Parse(line);

        // 2) Dump
        Console.WriteLine("AST DUMP:");
        FeelExpressionPrinter.Print(expr);
        Console.WriteLine();

        // 3) Eval
        var result = FeelExpressionEvaluator.Eval(expr, globals);

        // 4) Show
        if (result is Dictionary<string, object> dictionary && dictionary.TryGetValue("_globals", out object? value))
            globals = new Dictionary<string, object?>((Dictionary<string, object?>)value);
        if (result is IEnumerable<object> seq)
            Console.WriteLine("=> [" + string.Join(", ", seq) + "]");
        else
            Console.WriteLine("=> " + result);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}
