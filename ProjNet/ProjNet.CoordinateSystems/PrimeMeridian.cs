using System.Globalization;
using System.Text;

namespace ProjNet.CoordinateSystems;

public class PrimeMeridian : Info, IPrimeMeridian, IInfo
{
	private double _Longitude;

	private IAngularUnit _AngularUnit;

	public static PrimeMeridian Greenwich => new PrimeMeridian(0.0, ProjNet.CoordinateSystems.AngularUnit.Degrees, "Greenwich", "EPSG", 8901L, string.Empty, string.Empty, string.Empty);

	public static PrimeMeridian Lisbon => new PrimeMeridian(-9.0754862, ProjNet.CoordinateSystems.AngularUnit.Degrees, "Lisbon", "EPSG", 8902L, string.Empty, string.Empty, string.Empty);

	public static PrimeMeridian Paris => new PrimeMeridian(2.5969213, ProjNet.CoordinateSystems.AngularUnit.Degrees, "Paris", "EPSG", 8903L, string.Empty, string.Empty, "Value adopted by IGN (Paris) in 1936. Equivalent to 2 deg 20min 14.025sec. Preferred by EPSG to earlier value of 2deg 20min 13.95sec (2.596898 grads) used by RGS London.");

	public static PrimeMeridian Bogota => new PrimeMeridian(-74.04513, ProjNet.CoordinateSystems.AngularUnit.Degrees, "Bogota", "EPSG", 8904L, string.Empty, string.Empty, string.Empty);

	public static PrimeMeridian Madrid => new PrimeMeridian(-3.411658, ProjNet.CoordinateSystems.AngularUnit.Degrees, "Madrid", "EPSG", 8905L, string.Empty, string.Empty, string.Empty);

	public static PrimeMeridian Rome => new PrimeMeridian(12.27084, ProjNet.CoordinateSystems.AngularUnit.Degrees, "Rome", "EPSG", 8906L, string.Empty, string.Empty, string.Empty);

	public static PrimeMeridian Bern => new PrimeMeridian(7.26225, ProjNet.CoordinateSystems.AngularUnit.Degrees, "Bern", "EPSG", 8907L, string.Empty, string.Empty, "1895 value. Newer value of 7 deg 26 min 22.335 sec E determined in 1938.");

	public static PrimeMeridian Jakarta => new PrimeMeridian(106.482779, ProjNet.CoordinateSystems.AngularUnit.Degrees, "Jakarta", "EPSG", 8908L, string.Empty, string.Empty, string.Empty);

	public static PrimeMeridian Ferro => new PrimeMeridian(-17.4, ProjNet.CoordinateSystems.AngularUnit.Degrees, "Ferro", "EPSG", 8909L, string.Empty, string.Empty, "Used in Austria and former Czechoslovakia.");

	public static PrimeMeridian Brussels => new PrimeMeridian(4.220471, ProjNet.CoordinateSystems.AngularUnit.Degrees, "Brussels", "EPSG", 8910L, string.Empty, string.Empty, string.Empty);

	public static PrimeMeridian Stockholm => new PrimeMeridian(18.03298, ProjNet.CoordinateSystems.AngularUnit.Degrees, "Stockholm", "EPSG", 8911L, string.Empty, string.Empty, string.Empty);

	public static PrimeMeridian Athens => new PrimeMeridian(23.4258815, ProjNet.CoordinateSystems.AngularUnit.Degrees, "Athens", "EPSG", 8912L, string.Empty, string.Empty, "Used in Greece for older mapping based on Hatt projection.");

	public static PrimeMeridian Oslo => new PrimeMeridian(10.43225, ProjNet.CoordinateSystems.AngularUnit.Degrees, "Oslo", "EPSG", 8913L, string.Empty, string.Empty, "Formerly known as Kristiania or Christiania.");

	public double Longitude
	{
		get
		{
			return _Longitude;
		}
		set
		{
			_Longitude = value;
		}
	}

	public IAngularUnit AngularUnit
	{
		get
		{
			return _AngularUnit;
		}
		set
		{
			_AngularUnit = value;
		}
	}

	public override string WKT
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "PRIMEM[\"{0}\", {1}", new object[2] { base.Name, Longitude });
			if (!string.IsNullOrEmpty(base.Authority) && base.AuthorityCode > 0)
			{
				stringBuilder.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", base.Authority, base.AuthorityCode);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}

	public override string XML => string.Format(CultureInfo.InvariantCulture.NumberFormat, "<CS_PrimeMeridian Longitude=\"{0}\" >{1}{2}</CS_PrimeMeridian>", new object[3] { Longitude, base.InfoXml, AngularUnit.XML });

	internal PrimeMeridian(double longitude, IAngularUnit angularUnit, string name, string authority, long authorityCode, string alias, string abbreviation, string remarks)
		: base(name, authority, authorityCode, alias, abbreviation, remarks)
	{
		_Longitude = longitude;
		_AngularUnit = angularUnit;
	}

	public override bool EqualParams(object obj)
	{
		if (!(obj is PrimeMeridian))
		{
			return false;
		}
		PrimeMeridian primeMeridian = obj as PrimeMeridian;
		if (primeMeridian.AngularUnit.EqualParams(AngularUnit))
		{
			return primeMeridian.Longitude == Longitude;
		}
		return false;
	}
}
