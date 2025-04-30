namespace CsFeel.Internals.Exceptions;

public class TokenizerException(TokenizerExceptionType type, string? message)
    : Exception($"Tokenizer Error. TYPE:{type} MESSAGE:{message}")
{
    public TokenizerExceptionType Type { get; private set; } = type;
}

public enum TokenizerExceptionType
{
    INVALID_CHR,
}