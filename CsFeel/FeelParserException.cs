namespace CsFeel;

public class FeelParserException(FeelParserError errorCode, string message = "") : Exception
{
    private readonly FeelParserError _errorCode = errorCode;
    private readonly string _message = message;
    public string GetMessage => $"{_errorCode} {(string.IsNullOrEmpty(_message) ? $": {_message}" : "")}";
}