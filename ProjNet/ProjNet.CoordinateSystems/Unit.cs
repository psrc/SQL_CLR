using System;
using System.Globalization;
using System.Text;

namespace ProjNet.CoordinateSystems;

public class Unit : Info, IUnit, IInfo
{
	private double _ConversionFactor;

	public double ConversionFactor
	{
		get
		{
			return _ConversionFactor;
		}
		set
		{
			_ConversionFactor = value;
		}
	}

	public override string WKT
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "UNIT[\"{0}\", {1}", new object[2] { base.Name, _ConversionFactor });
			if (!string.IsNullOrEmpty(base.Authority) && base.AuthorityCode > 0)
			{
				stringBuilder.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", base.Authority, base.AuthorityCode);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}

	public override string XML
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal Unit(double conversionFactor, string name, string authority, long authorityCode, string alias, string abbreviation, string remarks)
		: base(name, authority, authorityCode, alias, abbreviation, remarks)
	{
		_ConversionFactor = conversionFactor;
	}

	internal Unit(string name, double conversionFactor)
		: this(conversionFactor, name, string.Empty, -1L, string.Empty, string.Empty, string.Empty)
	{
	}

	public override bool EqualParams(object obj)
	{
		if (!(obj is Unit))
		{
			return false;
		}
		return (obj as Unit).ConversionFactor == ConversionFactor;
	}
}
