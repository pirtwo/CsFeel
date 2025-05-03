using System.Text;
using System.Globalization;
using CsFeel.Internals.Exceptions;

namespace CsFeel.Internals;

public class Tokenizer
{
    public Token? CurrentToken => _token;
    public object? CurrentTokenValue => _tokenValue;

    protected readonly TextReader _reader;
    protected char _crrChar;
    protected Token? _token = null;
    protected object? _tokenValue = null;

    public Tokenizer(TextReader reader)
    {
        _reader = reader;

        NextChar();
        NextToken();
    }

    public void NextToken()
    {
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

            NextChar();

            return;
        }
        if (_crrChar == '-')
        {
            _token = Token.SUB;
            _tokenValue = null;

            NextChar();

            return;
        }
        if (_crrChar == '*' && LookAhead() != '*')
        {
            _token = Token.MUL;
            _tokenValue = null;

            NextChar();

            return;
        }
        if (_crrChar == '/')
        {
            _token = Token.DIV;
            _tokenValue = null;

            NextChar();

            return;
        }
        if (_crrChar == '*' && LookAhead() == '*')
        {
            _token = Token.EXP;
            _tokenValue = null;

            NextChar();
            NextChar();

            return;
        }
        if (_crrChar == '(')
        {
            _token = Token.PAR_OPE;
            _tokenValue = _crrChar.ToString();

            NextChar();

            return;
        }
        if (_crrChar == ')')
        {
            _token = Token.PAR_CLO;
            _tokenValue = _crrChar.ToString();

            NextChar();

            return;
        }

        // is variable ?
        if (char.IsLetter(_crrChar) || _crrChar == '_')
        {
            StringBuilder sb = new();

            while (char.IsLetter(_crrChar) || _crrChar == '_')
            {
                sb.Append(_crrChar);
                NextChar();
            }

            _token = Token.VAR;
            _tokenValue = sb.ToString();

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