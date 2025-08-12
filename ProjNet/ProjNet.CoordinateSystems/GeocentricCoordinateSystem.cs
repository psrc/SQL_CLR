using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ProjNet.CoordinateSystems;

public class GeocentricCoordinateSystem : CoordinateSystem, IGeocentricCoordinateSystem, ICoordinateSystem, IInfo
{
	private IHorizontalDatum _HorizontalDatum;

	private ILinearUnit _LinearUnit;

	private IPrimeMeridian _Primemeridan;

	public static IGeocentricCoordinateSystem WGS84 => new CoordinateSystemFactory().CreateGeocentricCoordinateSystem("WGS84 Geocentric", ProjNet.CoordinateSystems.HorizontalDatum.WGS84, ProjNet.CoordinateSystems.LinearUnit.Metre, ProjNet.CoordinateSystems.PrimeMeridian.Greenwich);

	public IHorizontalDatum HorizontalDatum
	{
		get
		{
			return _HorizontalDatum;
		}
		set
		{
			_HorizontalDatum = value;
		}
	}

	public ILinearUnit LinearUnit
	{
		get
		{
			return _LinearUnit;
		}
		set
		{
			_LinearUnit = value;
		}
	}

	public IPrimeMeridian PrimeMeridian
	{
		get
		{
			return _Primemeridan;
		}
		set
		{
			_Primemeridan = value;
		}
	}

	public override string WKT
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("GEOCCS[\"{0}\", {1}, {2}, {3}", base.Name, HorizontalDatum.WKT, PrimeMeridian.WKT, LinearUnit.WKT);
			if (base.AxisInfo.Count != 3 || base.AxisInfo[0].Name != "X" || base.AxisInfo[0].Orientation != 0 || base.AxisInfo[1].Name != "Y" || base.AxisInfo[1].Orientation != AxisOrientationEnum.East || base.AxisInfo[2].Name != "Z" || base.AxisInfo[2].Orientation != AxisOrientationEnum.North)
			{
				for (int i = 0; i < base.AxisInfo.Count; i++)
				{
					stringBuilder.AppendFormat(", {0}", GetAxis(i).WKT);
				}
			}
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
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "<CS_CoordinateSystem Dimension=\"{0}\"><CS_GeocentricCoordinateSystem>{1}", new object[2] { base.Dimension, base.InfoXml });
			foreach (AxisInfo item in base.AxisInfo)
			{
				stringBuilder.Append(item.XML);
			}
			stringBuilder.AppendFormat("{0}{1}{2}</CS_GeocentricCoordinateSystem></CS_CoordinateSystem>", HorizontalDatum.XML, LinearUnit.XML, PrimeMeridian.XML);
			return stringBuilder.ToString();
		}
	}

	internal GeocentricCoordinateSystem(IHorizontalDatum datum, ILinearUnit linearUnit, IPrimeMeridian primeMeridian, List<AxisInfo> axisinfo, string name, string authority, long code, string alias, string remarks, string abbreviation)
		: base(name, authority, code, alias, abbreviation, remarks)
	{
		_HorizontalDatum = datum;
		_LinearUnit = linearUnit;
		_Primemeridan = primeMeridian;
		if (axisinfo.Count != 3)
		{
			throw new ArgumentException("Axis info should contain three axes for geocentric coordinate systems");
		}
		base.AxisInfo = axisinfo;
	}

	public override IUnit GetUnits(int dimension)
	{
		return _LinearUnit;
	}

	public override bool EqualParams(object obj)
	{
		if (!(obj is GeocentricCoordinateSystem))
		{
			return false;
		}
		GeocentricCoordinateSystem geocentricCoordinateSystem = obj as GeocentricCoordinateSystem;
		if (geocentricCoordinateSystem.HorizontalDatum.EqualParams(HorizontalDatum) && geocentricCoordinateSystem.LinearUnit.EqualParams(LinearUnit))
		{
			return geocentricCoordinateSystem.PrimeMeridian.EqualParams(PrimeMeridian);
		}
		return false;
	}
}
