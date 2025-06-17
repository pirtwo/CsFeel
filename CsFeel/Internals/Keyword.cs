namespace CsFeel.Internals;

public enum Keyword
{
    NULL,
    TRUE,
    FALSE,
    AND,
    OR,
    IF,
    ELSE,
    THEN,
    SOME,
    IN,
    SATISFIES,
    BETWEEN,
}

public static class KeywordExtensions
{
    public static Keyword? KeywordTest(this string word) => word switch
    {
        "null" => Keyword.NULL,
        "true" => Keyword.TRUE,
        "false" => Keyword.FALSE,
        "and" => Keyword.AND,
        "or" => Keyword.OR,
        "if" => Keyword.IF,
        "else" => Keyword.ELSE,
        "then" => Keyword.THEN,
        "between" => Keyword.BETWEEN,
        "some" => Keyword.SOME,
        "in" => Keyword.IN,
        "satisfies" => Keyword.SATISFIES,
        _ => null,
    };

}