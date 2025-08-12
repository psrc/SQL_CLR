using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace ProjNet.Converters.WellKnownText.IO;

internal class StreamTokenizer
{
	private TokenType _currentTokenType;

	private TextReader _reader;

	private string _currentToken;

	private bool _ignoreWhitespace;

	private int _lineNumber = 1;

	private int _colNumber = 1;

	public int LineNumber => _lineNumber;

	public int Column => _colNumber;

	public StreamTokenizer(TextReader reader, bool ignoreWhitespace)
	{
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
		_reader = reader;
		_ignoreWhitespace = ignoreWhitespace;
	}

	public double GetNumericValue()
	{
		string stringValue = GetStringValue();
		if (GetTokenType() == TokenType.Number)
		{
			return double.Parse(stringValue, CultureInfo.InvariantCulture.NumberFormat);
		}
		throw new ArgumentException(string.Format(CultureInfo.InvariantCulture.NumberFormat, "The token '{0}' is not a number at line {1} column {2}.", new object[3] { stringValue, LineNumber, Column }));
	}

	public string GetStringValue()
	{
		return _currentToken;
	}

	public TokenType GetTokenType()
	{
		return _currentTokenType;
	}

	public TokenType NextToken(bool ignoreWhitespace)
	{
		if (ignoreWhitespace)
		{
			return NextNonWhitespaceToken();
		}
		return NextTokenAny();
	}

	public TokenType NextToken()
	{
		return NextToken(_ignoreWhitespace);
	}

	private TokenType NextTokenAny()
	{
		TokenType tokenType = TokenType.Eof;
		char[] array = new char[1];
		_currentToken = "";
		_currentTokenType = TokenType.Eof;
		int num = _reader.Read(array, 0, 1);
		bool flag = false;
		bool flag2 = false;
		byte[] array2 = null;
		ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();
		char[] array3 = null;
		while (num != 0)
		{
			array2 = new byte[1] { (byte)_reader.Peek() };
			array3 = aSCIIEncoding.GetChars(array2);
			char c = array[0];
			char c2 = array3[0];
			_currentTokenType = GetType(c);
			tokenType = GetType(c2);
			if (flag2 && c == '_')
			{
				_currentTokenType = TokenType.Word;
			}
			if (flag2 && _currentTokenType == TokenType.Number)
			{
				_currentTokenType = TokenType.Word;
			}
			if (_currentTokenType == TokenType.Word && c2 == '_')
			{
				tokenType = TokenType.Word;
				flag2 = true;
			}
			if (_currentTokenType == TokenType.Word && tokenType == TokenType.Number)
			{
				tokenType = TokenType.Word;
				flag2 = true;
			}
			if (c == '-' && tokenType == TokenType.Number && !flag)
			{
				_currentTokenType = TokenType.Number;
				tokenType = TokenType.Number;
			}
			if (flag && tokenType == TokenType.Number && c == '.')
			{
				_currentTokenType = TokenType.Number;
			}
			if (_currentTokenType == TokenType.Number && c2 == '.' && !flag)
			{
				tokenType = TokenType.Number;
				flag = true;
			}
			_colNumber++;
			if (_currentTokenType == TokenType.Eol)
			{
				_lineNumber++;
				_colNumber = 1;
			}
			_currentToken += c;
			num = ((_currentTokenType == tokenType) ? ((_currentTokenType != TokenType.Symbol || c == '-') ? _reader.Read(array, 0, 1) : 0) : 0);
		}
		return _currentTokenType;
	}

	private static TokenType GetType(char character)
	{
		if (char.IsDigit(character))
		{
			return TokenType.Number;
		}
		if (char.IsLetter(character))
		{
			return TokenType.Word;
		}
		if (character == '\n')
		{
			return TokenType.Eol;
		}
		if (char.IsWhiteSpace(character) || char.IsControl(character))
		{
			return TokenType.Whitespace;
		}
		return TokenType.Symbol;
	}

	private TokenType NextNonWhitespaceToken()
	{
		TokenType tokenType = NextTokenAny();
		while (tokenType == TokenType.Whitespace || tokenType == TokenType.Eol)
		{
			tokenType = NextTokenAny();
		}
		return tokenType;
	}
}
