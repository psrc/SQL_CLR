using System;
using System.Globalization;
using System.Text;

namespace ProjNet.CoordinateSystems;

public class AngularUnit : Info, IAngularUnit, IUnit, IInfo
{
	private double _RadiansPerUnit;

	public static AngularUnit Degrees => new AngularUnit(Math.PI / 180.0, "degree", "EPSG", 9102L, "deg", string.Empty, "=pi/180 radians");

	public static AngularUnit Radian => new AngularUnit(1.0, "radian", "EPSG", 9101L, "rad", string.Empty, "SI standard unit.");

	public static AngularUnit Grad => new AngularUnit(Math.PI / 200.0, "grad", "EPSG", 9105L, "gr", string.Empty, "=pi/200 radians.");

	public static AngularUnit Gon => new AngularUnit(Math.PI / 200.0, "gon", "EPSG", 9106L, "g", string.Empty, "=pi/200 radians.");

	public double RadiansPerUnit
	{
		get
		{
			return _RadiansPerUnit;
		}
		set
		{
			_RadiansPerUnit = value;
		}
	}

	public override string WKT
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "UNIT[\"{0}\", {1}", new object[2] { base.Name, RadiansPerUnit });
			if (!string.IsNullOrEmpty(base.Authority) && base.AuthorityCode > 0)
			{
				stringBuilder.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", base.Authority, base.AuthorityCode);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}

	public override string XML => string.Format(CultureInfo.InvariantCulture.NumberFormat, "<CS_AngularUnit RadiansPerUnit=\"{0}\">{1}</CS_AngularUnit>", new object[2] { RadiansPerUnit, base.InfoXml });

	public AngularUnit(double radiansPerUnit)
		: this(radiansPerUnit, string.Empty, string.Empty, -1L, string.Empty, string.Empty, string.Empty)
	{
	}

	internal AngularUnit(double radiansPerUnit, string name, string authority, long authorityCode, string alias, string abbreviation, string remarks)
		: base(name, authority, authorityCode, alias, abbreviation, remarks)
	{
		_RadiansPerUnit = radiansPerUnit;
	}

	public override bool EqualParams(object obj)
	{
		if (!(obj is AngularUnit))
		{
			return false;
		}
		return (obj as AngularUnit).RadiansPerUnit == RadiansPerUnit;
	}
}
