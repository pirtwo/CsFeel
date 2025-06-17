using System.Text;
using System.Globalization;
using CsFeel.Internals.Exceptions;

namespace CsFeel.Internals;

public class Tokenizer
{
    protected readonly TextReader _reader;
    protected char _crrChar;
    protected Token? _token = null;

    protected bool _bool = false;
    protected decimal _number = 0;
    protected string _string = string.Empty;
    protected string _identifier = string.Empty;
    protected Keyword? _keyword = null;

    public Token? CurrentToken => _token;
    public bool GetBool => _bool;
    public decimal GetNumber => _number;
    public string GetString => _string;
    public string GetIdentifier => _identifier;
    public Keyword? GetKeyword => _keyword;

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
            return;
        }
        if (_crrChar == '+')
        {
            _token = Token.ADD;

            NextChar();

            return;
        }
        if (_crrChar == '-')
        {
            _token = Token.SUB;

            NextChar();

            return;
        }
        if (_crrChar == '*' && LookAhead() != '*')
        {
            _token = Token.MUL;

            NextChar();

            return;
        }
        if (_crrChar == '/')
        {
            _token = Token.DIV;

            NextChar();

            return;
        }
        if (_crrChar == '*' && LookAhead() == '*')
        {
            _token = Token.POW;

            NextChar();
            NextChar();

            return;
        }
        if (_crrChar == '(')
        {
            _token = Token.PAR_OPE;

            NextChar();

            return;
        }
        if (_crrChar == ')')
        {
            _token = Token.PAR_CLO;

            NextChar();

            return;
        }
        if (_crrChar == '[')
        {
            _token = Token.BRK_OPE;

            NextChar();

            return;
        }
        if (_crrChar == ']')
        {
            _token = Token.BRK_CLO;

            NextChar();

            return;
        }
        if (_crrChar == '=')
        {
            _token = Token.CMP_EQ;

            NextChar();

            return;
        }
        if (_crrChar == '!' && LookAhead() == '=')
        {
            _token = Token.CMP_NEQ;

            NextChar();
            NextChar();

            return;
        }
        if (_crrChar == '<' && LookAhead() == '=')
        {
            _token = Token.CMP_LTE;

            NextChar();
            NextChar();

            return;
        }
        if (_crrChar == '<')
        {
            _token = Token.CMP_LT;

            NextChar();

            return;
        }
        if (_crrChar == '>' && LookAhead() == '=')
        {
            _token = Token.CMP_GTE;

            NextChar();
            NextChar();

            return;
        }
        if (_crrChar == '>')
        {
            _token = Token.CMP_GT;

            NextChar();

            return;
        }
        if (_crrChar == ',')
        {
            _token = Token.COMMA;

            NextChar();

            return;
        }

        // is keyword or variable ?
        if (char.IsLetter(_crrChar) || _crrChar == '_')
        {
            StringBuilder sb = new();

            while (char.IsLetter(_crrChar) || _crrChar == '_')
            {
                sb.Append(_crrChar);
                NextChar();
            }

            var word = sb.ToString();

            // is keywords ?
            if (word.KeywordTest() != null)
            {
                _token = Token.KWD;
                _keyword = word.KeywordTest();
            }
            else
            {
                _token = Token.VAR;
                _identifier = sb.ToString();
            }

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
            _number = decimal.Parse(sb.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture);

            return;
        }

        // is string ?
        if (_crrChar == '"')
        {
            // skip
            NextChar();

            StringBuilder sb = new();

            while (_crrChar != '"')
            {
                sb.Append(_crrChar);
                NextChar();
            }

            //skip
            NextChar();

            _string = sb.ToString();
            _token = Token.STR;

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