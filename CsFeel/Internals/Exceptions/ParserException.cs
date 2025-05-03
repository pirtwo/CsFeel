namespace CsFeel.Internals.Exceptions;

public class ParserException(ParserExceptionType type, string? message)
    : Exception($"Parser Error. TYPE:{type} MESSAGE:{message}")
{
    public ParserExceptionType Type { get; private set; } = type;
}

public enum ParserExceptionType
{
    SYNTAX_ERROR,
    INVALID_TOKEN,
    INVALID_TOKEN_VALUE,
    NULL_VARIABLE_NAME,
}