using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ProjNet.CoordinateSystems;

public class GeographicCoordinateSystem : HorizontalCoordinateSystem, IGeographicCoordinateSystem, IHorizontalCoordinateSystem, ICoordinateSystem, IInfo
{
	private IAngularUnit _AngularUnit;

	private IPrimeMeridian _PrimeMeridian;

	private List<Wgs84ConversionInfo> _WGS84ConversionInfo;

	public static GeographicCoordinateSystem WGS84
	{
		get
		{
			List<AxisInfo> list = new List<AxisInfo>(2);
			list.Add(new AxisInfo("Lon", AxisOrientationEnum.East));
			list.Add(new AxisInfo("Lat", AxisOrientationEnum.North));
			return new GeographicCoordinateSystem(ProjNet.CoordinateSystems.AngularUnit.Degrees, ProjNet.CoordinateSystems.HorizontalDatum.WGS84, ProjNet.CoordinateSystems.PrimeMeridian.Greenwich, list, "WGS 84", "EPSG", 4326L, string.Empty, string.Empty, string.Empty);
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

	public IPrimeMeridian PrimeMeridian
	{
		get
		{
			return _PrimeMeridian;
		}
		set
		{
			_PrimeMeridian = value;
		}
	}

	public int NumConversionToWGS84 => _WGS84ConversionInfo.Count;

	internal List<Wgs84ConversionInfo> WGS84ConversionInfo
	{
		get
		{
			return _WGS84ConversionInfo;
		}
		set
		{
			_WGS84ConversionInfo = value;
		}
	}

	public override string WKT
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("GEOGCS[\"{0}\", {1}, {2}, {3}", base.Name, base.HorizontalDatum.WKT, PrimeMeridian.WKT, AngularUnit.WKT);
			if (base.AxisInfo.Count != 2 || base.AxisInfo[0].Name != "Lon" || base.AxisInfo[0].Orientation != AxisOrientationEnum.East || base.AxisInfo[1].Name != "Lat" || base.AxisInfo[1].Orientation != AxisOrientationEnum.North)
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
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "<CS_CoordinateSystem Dimension=\"{0}\"><CS_GeographicCoordinateSystem>{1}", new object[2] { base.Dimension, base.InfoXml });
			foreach (AxisInfo item in base.AxisInfo)
			{
				stringBuilder.Append(item.XML);
			}
			stringBuilder.AppendFormat("{0}{1}{2}</CS_GeographicCoordinateSystem></CS_CoordinateSystem>", base.HorizontalDatum.XML, AngularUnit.XML, PrimeMeridian.XML);
			return stringBuilder.ToString();
		}
	}

	internal GeographicCoordinateSystem(IAngularUnit angularUnit, IHorizontalDatum horizontalDatum, IPrimeMeridian primeMeridian, List<AxisInfo> axisInfo, string name, string authority, long authorityCode, string alias, string abbreviation, string remarks)
		: base(horizontalDatum, axisInfo, name, authority, authorityCode, alias, abbreviation, remarks)
	{
		_AngularUnit = angularUnit;
		_PrimeMeridian = primeMeridian;
	}

	public override IUnit GetUnits(int dimension)
	{
		return _AngularUnit;
	}

	public Wgs84ConversionInfo GetWgs84ConversionInfo(int index)
	{
		return _WGS84ConversionInfo[index];
	}

	public override bool EqualParams(object obj)
	{
		if (!(obj is GeographicCoordinateSystem))
		{
			return false;
		}
		GeographicCoordinateSystem geographicCoordinateSystem = obj as GeographicCoordinateSystem;
		if (geographicCoordinateSystem.Dimension != base.Dimension)
		{
			return false;
		}
		if (WGS84ConversionInfo != null && geographicCoordinateSystem.WGS84ConversionInfo == null)
		{
			return false;
		}
		if (WGS84ConversionInfo == null && geographicCoordinateSystem.WGS84ConversionInfo != null)
		{
			return false;
		}
		if (WGS84ConversionInfo != null && geographicCoordinateSystem.WGS84ConversionInfo != null)
		{
			if (WGS84ConversionInfo.Count != geographicCoordinateSystem.WGS84ConversionInfo.Count)
			{
				return false;
			}
			for (int i = 0; i < WGS84ConversionInfo.Count; i++)
			{
				if (!geographicCoordinateSystem.WGS84ConversionInfo[i].Equals(WGS84ConversionInfo[i]))
				{
					return false;
				}
			}
		}
		if (base.AxisInfo.Count != geographicCoordinateSystem.AxisInfo.Count)
		{
			return false;
		}
		for (int j = 0; j < geographicCoordinateSystem.AxisInfo.Count; j++)
		{
			if (geographicCoordinateSystem.AxisInfo[j].Orientation != base.AxisInfo[j].Orientation)
			{
				return false;
			}
		}
		if (geographicCoordinateSystem.AngularUnit.EqualParams(AngularUnit) && geographicCoordinateSystem.HorizontalDatum.EqualParams(base.HorizontalDatum))
		{
			return geographicCoordinateSystem.PrimeMeridian.EqualParams(PrimeMeridian);
		}
		return false;
	}
}
