using System.Globalization;
using System.Text;

namespace ProjNet.CoordinateSystems;

public class Ellipsoid : Info, IEllipsoid, IInfo
{
	private double _SemiMajorAxis;

	private double _SemiMinorAxis;

	private double _InverseFlattening;

	private ILinearUnit _AxisUnit;

	private bool _IsIvfDefinitive;

	public static Ellipsoid WGS84 => new Ellipsoid(6378137.0, 0.0, 298.257223563, isIvfDefinitive: true, LinearUnit.Metre, "WGS 84", "EPSG", 7030L, "WGS84", "", "Inverse flattening derived from four defining parameters (semi-major axis; C20 = -484.16685*10e-6; earth's angular velocity w = 7292115e11 rad/sec; gravitational constant GM = 3986005e8 m*m*m/s/s).");

	public static Ellipsoid WGS72 => new Ellipsoid(6378135.0, 0.0, 298.26, isIvfDefinitive: true, LinearUnit.Metre, "WGS 72", "EPSG", 7043L, "WGS 72", string.Empty, string.Empty);

	public static Ellipsoid GRS80 => new Ellipsoid(6378137.0, 0.0, 298.257222101, isIvfDefinitive: true, LinearUnit.Metre, "GRS 1980", "EPSG", 7019L, "International 1979", "", "Adopted by IUGG 1979 Canberra.  Inverse flattening is derived from geocentric gravitational constant GM = 3986005e8 m*m*m/s/s; dynamic form factor J2 = 108263e8 and Earth's angular velocity = 7292115e-11 rad/s.");

	public static Ellipsoid International1924 => new Ellipsoid(6378388.0, 0.0, 297.0, isIvfDefinitive: true, LinearUnit.Metre, "International 1924", "EPSG", 7022L, "Hayford 1909", string.Empty, "Described as a=6378388 m. and b=6356909 m. from which 1/f derived to be 296.95926. The figure was adopted as the International ellipsoid in 1924 but with 1/f taken as 297 exactly from which b is derived as 6356911.946m.");

	public static Ellipsoid Clarke1880 => new Ellipsoid(20926202.0, 0.0, 297.0, isIvfDefinitive: true, LinearUnit.ClarkesFoot, "Clarke 1880", "EPSG", 7034L, "Clarke 1880", string.Empty, "Clarke gave a and b and also 1/f=293.465 (to 3 decimal places).  1/f derived from a and b = 293.4663077…");

	public static Ellipsoid Clarke1866 => new Ellipsoid(6378206.4, 6356583.8, double.PositiveInfinity, isIvfDefinitive: false, LinearUnit.Metre, "Clarke 1866", "EPSG", 7008L, "Clarke 1866", string.Empty, "Original definition a=20926062 and b=20855121 (British) feet. Uses Clarke's 1865 inch-metre ratio of 39.370432 to obtain metres. (Metric value then converted to US survey feet for use in the United States using 39.37 exactly giving a=20925832.16 ft US).");

	public static Ellipsoid Sphere => new Ellipsoid(6370997.0, 6370997.0, double.PositiveInfinity, isIvfDefinitive: false, LinearUnit.Metre, "GRS 1980 Authalic Sphere", "EPSG", 7048L, "Sphere", "", "Authalic sphere derived from GRS 1980 ellipsoid (code 7019).  (An authalic sphere is one with a surface area equal to the surface area of the ellipsoid). 1/f is infinite.");

	public double SemiMajorAxis
	{
		get
		{
			return _SemiMajorAxis;
		}
		set
		{
			_SemiMajorAxis = value;
		}
	}

	public double SemiMinorAxis
	{
		get
		{
			return _SemiMinorAxis;
		}
		set
		{
			_SemiMinorAxis = value;
		}
	}

	public double InverseFlattening
	{
		get
		{
			return _InverseFlattening;
		}
		set
		{
			_InverseFlattening = value;
		}
	}

	public ILinearUnit AxisUnit
	{
		get
		{
			return _AxisUnit;
		}
		set
		{
			_AxisUnit = value;
		}
	}

	public bool IsIvfDefinitive
	{
		get
		{
			return _IsIvfDefinitive;
		}
		set
		{
			_IsIvfDefinitive = value;
		}
	}

	public override string WKT
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "SPHEROID[\"{0}\", {1}, {2}", new object[3] { base.Name, SemiMajorAxis, InverseFlattening });
			if (!string.IsNullOrEmpty(base.Authority) && base.AuthorityCode > 0)
			{
				stringBuilder.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", base.Authority, base.AuthorityCode);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}

	public override string XML => string.Format(CultureInfo.InvariantCulture.NumberFormat, "<CS_Ellipsoid SemiMajorAxis=\"{0}\" SemiMinorAxis=\"{1}\" InverseFlattening=\"{2}\" IvfDefinitive=\"{3}\">{4}{5}</CS_Ellipsoid>", SemiMajorAxis, SemiMinorAxis, InverseFlattening, IsIvfDefinitive ? 1 : 0, base.InfoXml, AxisUnit.XML);

	internal Ellipsoid(double semiMajorAxis, double semiMinorAxis, double inverseFlattening, bool isIvfDefinitive, ILinearUnit axisUnit, string name, string authority, long code, string alias, string abbreviation, string remarks)
		: base(name, authority, code, alias, abbreviation, remarks)
	{
		_SemiMajorAxis = semiMajorAxis;
		_InverseFlattening = inverseFlattening;
		_AxisUnit = axisUnit;
		_IsIvfDefinitive = isIvfDefinitive;
		if (isIvfDefinitive && (inverseFlattening == 0.0 || double.IsInfinity(inverseFlattening)))
		{
			_SemiMinorAxis = semiMajorAxis;
		}
		else if (isIvfDefinitive)
		{
			_SemiMinorAxis = (1.0 - 1.0 / _InverseFlattening) * semiMajorAxis;
		}
		else
		{
			_SemiMinorAxis = semiMinorAxis;
		}
	}

	public override bool EqualParams(object obj)
	{
		if (!(obj is Ellipsoid))
		{
			return false;
		}
		Ellipsoid ellipsoid = obj as Ellipsoid;
		if (ellipsoid.InverseFlattening == InverseFlattening && ellipsoid.IsIvfDefinitive == IsIvfDefinitive && ellipsoid.SemiMajorAxis == SemiMajorAxis && ellipsoid.SemiMinorAxis == SemiMinorAxis)
		{
			return ellipsoid.AxisUnit.EqualParams(AxisUnit);
		}
		return false;
	}
}
