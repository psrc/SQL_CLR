using System.Globalization;
using System.Text;

namespace ProjNet.CoordinateSystems;

public class LinearUnit : Info, ILinearUnit, IUnit, IInfo
{
	private double _MetersPerUnit;

	public static ILinearUnit Metre => new LinearUnit(1.0, "metre", "EPSG", 9001L, "m", string.Empty, "Also known as International metre. SI standard unit.");

	public static ILinearUnit Foot => new LinearUnit(0.3048, "foot", "EPSG", 9002L, "ft", string.Empty, string.Empty);

	public static ILinearUnit USSurveyFoot => new LinearUnit(0.304800609601219, "US survey foot", "EPSG", 9003L, "American foot", "ftUS", "Used in USA.");

	public static ILinearUnit NauticalMile => new LinearUnit(1852.0, "nautical mile", "EPSG", 9030L, "NM", string.Empty, string.Empty);

	public static ILinearUnit ClarkesFoot => new LinearUnit(0.3047972654, "Clarke's foot", "EPSG", 9005L, "Clarke's foot", string.Empty, "Assumes Clarke's 1865 ratio of 1 British foot = 0.3047972654 French legal metres applies to the international metre. Used in older Australian, southern African & British West Indian mapping.");

	public double MetersPerUnit
	{
		get
		{
			return _MetersPerUnit;
		}
		set
		{
			_MetersPerUnit = value;
		}
	}

	public override string WKT
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "UNIT[\"{0}\", {1}", new object[2] { base.Name, MetersPerUnit });
			if (!string.IsNullOrEmpty(base.Authority) && base.AuthorityCode > 0)
			{
				stringBuilder.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", base.Authority, base.AuthorityCode);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}

	public override string XML => string.Format(CultureInfo.InvariantCulture.NumberFormat, "<CS_LinearUnit MetersPerUnit=\"{0}\">{1}</CS_LinearUnit>", new object[2] { MetersPerUnit, base.InfoXml });

	public LinearUnit(double metersPerUnit, string name, string authority, long authorityCode, string alias, string abbreviation, string remarks)
		: base(name, authority, authorityCode, alias, abbreviation, remarks)
	{
		_MetersPerUnit = metersPerUnit;
	}

	public override bool EqualParams(object obj)
	{
		if (!(obj is LinearUnit))
		{
			return false;
		}
		return (obj as LinearUnit).MetersPerUnit == MetersPerUnit;
	}
}
