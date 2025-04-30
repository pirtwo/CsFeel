using System.Text;
using System.Globalization;
using CsFeel.Internals.Exceptions;

namespace CsFeel.Internals;

public class Tokenizer(TextReader reader)
{
    protected readonly TextReader _reader = reader;
    protected char _crrChar;
    protected Token? _token = null;
    protected object? _tokenValue = null;

    public Token? CurrentToken => _token;
    public object? CurrentTokenValue => _tokenValue;

    public void NextToken()
    {
        NextChar();

        // skip white spaces
        while (char.IsWhiteSpace(_crrChar))
        {
            NextChar();
        }

        // is special ?
        if (_crrChar == '\0')
        {
            _token = Token.EOF;
            _tokenValue = null;
            return;
        }
        if (_crrChar == '+')
        {
            _token = Token.ADD;
            _tokenValue = null;
            return;
        }
        if (_crrChar == '-')
        {
            _token = Token.SUB;
            _tokenValue = null;
            return;
        }
        if (_crrChar == '*' && LookAhead() != '*')
        {
            _token = Token.MUL;
            _tokenValue = null;
            return;
        }
        if (_crrChar == '/')
        {
            _token = Token.DIV;
            _tokenValue = null;
            return;
        }
        if (_crrChar == '*' && LookAhead() == '*')
        {
            _token = Token.EXP;
            _tokenValue = null;

            // move to next char
            NextChar();

            return;
        }

        // is Number ?
        if (char.IsDigit(_crrChar) || _crrChar == '.')
        {
            StringBuilder sb = new();

            var hasDecPoint = false;
            while (char.IsDigit(_crrChar) || (_crrChar == '.' && !hasDecPoint))
            {
                sb.Append(_crrChar);
                hasDecPoint = _crrChar == '.';
                NextChar();
            }

            _token = Token.NUM;
            _tokenValue = decimal.Parse(sb.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);
            return;
        }

        // invalid char 
        throw new TokenizerException(TokenizerExceptionType.INVALID_CHR, $"invalid char {_crrChar}");
    }

    protected void NextChar()
    {
        int ch = _reader.Read();
        _crrChar = ch < 0 ? '\0' : (char)ch;
    }

    protected char LookAhead()
    {
        int ch = _reader.Peek();
        return ch < 0 ? '\0' : (char)ch;
    }
}