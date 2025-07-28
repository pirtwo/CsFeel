namespace CsFeel;

public static class Helper
{
    public static bool IsEqual(object? a, object? b) => (a, b) switch
    {
        var (x, y) when x is null && y is null => true,
        var (x, y) when x is bool xb && y is bool yb => xb == yb,
        var (x, y) when x is string xs && y is string ys => xs == ys,
        var (x, y) when x is decimal xn && y is decimal yn => xn == yn,
        var (x, y) when x is DateTime xd && y is DateTime yd => xd == yd,
        var (x, y) when x is List<object?> xl && y is List<object?> yl => xl == yl,
        _ => false,
    };

    public static bool IsEqual(List<object?> a, List<object?> b)
        => a.Count == b.Count && a.Select((x, i) => new { Index = i, Item = x }).All(x => IsEqual(x.Item, b[x.Index]));

}