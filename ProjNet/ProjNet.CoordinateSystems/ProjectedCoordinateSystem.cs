using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ProjNet.CoordinateSystems;

public class ProjectedCoordinateSystem : HorizontalCoordinateSystem, IProjectedCoordinateSystem, IHorizontalCoordinateSystem, ICoordinateSystem, IInfo
{
	private IGeographicCoordinateSystem _GeographicCoordinateSystem;

	private ILinearUnit _LinearUnit;

	private IProjection _Projection;

	public IGeographicCoordinateSystem GeographicCoordinateSystem
	{
		get
		{
			return _GeographicCoordinateSystem;
		}
		set
		{
			_GeographicCoordinateSystem = value;
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

	public IProjection Projection
	{
		get
		{
			return _Projection;
		}
		set
		{
			_Projection = value;
		}
	}

	public override string WKT
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("PROJCS[\"{0}\", {1}, {2}", base.Name, GeographicCoordinateSystem.WKT, Projection.WKT);
			for (int i = 0; i < Projection.NumParameters; i++)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, ", {0}", new object[1] { Projection.GetParameter(i).WKT });
			}
			stringBuilder.AppendFormat(", {0}", LinearUnit.WKT);
			if (base.AxisInfo.Count != 2 || base.AxisInfo[0].Name != "X" || base.AxisInfo[0].Orientation != AxisOrientationEnum.East || base.AxisInfo[1].Name != "Y" || base.AxisInfo[1].Orientation != AxisOrientationEnum.North)
			{
				for (int j = 0; j < base.AxisInfo.Count; j++)
				{
					stringBuilder.AppendFormat(", {0}", GetAxis(j).WKT);
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
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "<CS_CoordinateSystem Dimension=\"{0}\"><CS_ProjectedCoordinateSystem>{1}", new object[2] { base.Dimension, base.InfoXml });
			foreach (AxisInfo item in base.AxisInfo)
			{
				stringBuilder.Append(item.XML);
			}
			stringBuilder.AppendFormat("{0}{1}{2}</CS_ProjectedCoordinateSystem></CS_CoordinateSystem>", GeographicCoordinateSystem.XML, LinearUnit.XML, Projection.XML);
			return stringBuilder.ToString();
		}
	}

	internal ProjectedCoordinateSystem(IHorizontalDatum datum, IGeographicCoordinateSystem geographicCoordinateSystem, ILinearUnit linearUnit, IProjection projection, List<AxisInfo> axisInfo, string name, string authority, long code, string alias, string remarks, string abbreviation)
		: base(datum, axisInfo, name, authority, code, alias, abbreviation, remarks)
	{
		_GeographicCoordinateSystem = geographicCoordinateSystem;
		_LinearUnit = linearUnit;
		_Projection = projection;
	}

	public static ProjectedCoordinateSystem WGS84_UTM(int Zone, bool ZoneIsNorth)
	{
		List<ProjectionParameter> list = new List<ProjectionParameter>();
		list.Add(new ProjectionParameter("latitude_of_origin", 0.0));
		list.Add(new ProjectionParameter("central_meridian", Zone * 6 - 183));
		list.Add(new ProjectionParameter("scale_factor", 0.9996));
		list.Add(new ProjectionParameter("false_easting", 500000.0));
		list.Add(new ProjectionParameter("false_northing", (!ZoneIsNorth) ? 10000000 : 0));
		Projection projection = new Projection("Transverse_Mercator", list, "UTM" + Zone.ToString(CultureInfo.InvariantCulture) + (ZoneIsNorth ? "N" : "S"), "EPSG", 32600 + Zone + ((!ZoneIsNorth) ? 100 : 0), string.Empty, string.Empty, string.Empty);
		List<AxisInfo> list2 = new List<AxisInfo>();
		list2.Add(new AxisInfo("East", AxisOrientationEnum.East));
		list2.Add(new AxisInfo("North", AxisOrientationEnum.North));
		return new ProjectedCoordinateSystem(ProjNet.CoordinateSystems.HorizontalDatum.WGS84, ProjNet.CoordinateSystems.GeographicCoordinateSystem.WGS84, ProjNet.CoordinateSystems.LinearUnit.Metre, projection, list2, "WGS 84 / UTM zone " + Zone.ToString(CultureInfo.InvariantCulture) + (ZoneIsNorth ? "N" : "S"), "EPSG", 32600 + Zone + ((!ZoneIsNorth) ? 100 : 0), string.Empty, "Large and medium scale topographic mapping and engineering survey.", string.Empty);
	}

	public override IUnit GetUnits(int dimension)
	{
		return _LinearUnit;
	}

	public override bool EqualParams(object obj)
	{
		if (!(obj is ProjectedCoordinateSystem))
		{
			return false;
		}
		ProjectedCoordinateSystem projectedCoordinateSystem = obj as ProjectedCoordinateSystem;
		if (projectedCoordinateSystem.Dimension != base.Dimension)
		{
			return false;
		}
		for (int i = 0; i < projectedCoordinateSystem.Dimension; i++)
		{
			if (projectedCoordinateSystem.GetAxis(i).Orientation != GetAxis(i).Orientation)
			{
				return false;
			}
			if (!projectedCoordinateSystem.GetUnits(i).EqualParams(GetUnits(i)))
			{
				return false;
			}
		}
		if (projectedCoordinateSystem.GeographicCoordinateSystem.EqualParams(GeographicCoordinateSystem) && projectedCoordinateSystem.HorizontalDatum.EqualParams(base.HorizontalDatum) && projectedCoordinateSystem.LinearUnit.EqualParams(LinearUnit))
		{
			return projectedCoordinateSystem.Projection.EqualParams(Projection);
		}
		return false;
	}
}
