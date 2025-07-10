using CsFeel;
using Sprache;

Console.WriteLine("FEEL REPL - type 'exit' to quit");
var globals = new Dictionary<string, object>();

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
        var result = FeelExpressionEval.Eval(expr, globals);

        // 4) Show
        if (result is Dictionary<string, object> dictionary)
            globals = new Dictionary<string, object>(dictionary);
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
