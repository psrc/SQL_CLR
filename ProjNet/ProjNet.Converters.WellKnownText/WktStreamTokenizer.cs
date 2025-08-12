using System;
using System.Globalization;
using System.IO;
using ProjNet.Converters.WellKnownText.IO;

namespace ProjNet.Converters.WellKnownText;

internal class WktStreamTokenizer : StreamTokenizer
{
	public WktStreamTokenizer(TextReader reader)
		: base(reader, ignoreWhitespace: true)
	{
		if (reader == null)
		{
			throw new ArgumentNullException("reader");
		}
	}

	internal void ReadToken(string expectedToken)
	{
		NextToken();
		if (GetStringValue() != expectedToken)
		{
			throw new ArgumentException(string.Format(CultureInfo.InvariantCulture.NumberFormat, "Expecting ('{3}') but got a '{0}' at line {1} column {2}.", GetStringValue(), base.LineNumber, base.Column, expectedToken));
		}
	}

	public string ReadDoubleQuotedWord()
	{
		string text = "";
		ReadToken("\"");
		NextToken(ignoreWhitespace: false);
		while (GetStringValue() != "\"")
		{
			text += GetStringValue();
			NextToken(ignoreWhitespace: false);
		}
		return text;
	}

	public void ReadAuthority(ref string authority, ref long authorityCode)
	{
		if (GetStringValue() != "AUTHORITY")
		{
			ReadToken("AUTHORITY");
		}
		ReadToken("[");
		authority = ReadDoubleQuotedWord();
		ReadToken(",");
		long.TryParse(ReadDoubleQuotedWord(), NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out authorityCode);
		ReadToken("]");
	}
}
