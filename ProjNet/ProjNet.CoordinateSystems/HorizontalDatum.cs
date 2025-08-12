using System.Globalization;
using System.Text;

namespace ProjNet.CoordinateSystems;

public class HorizontalDatum : Datum, IHorizontalDatum, IDatum, IInfo
{
	private IEllipsoid _Ellipsoid;

	private Wgs84ConversionInfo _Wgs84ConversionInfo;

	public static HorizontalDatum WGS84 => new HorizontalDatum(ProjNet.CoordinateSystems.Ellipsoid.WGS84, null, DatumType.HD_Geocentric, "World Geodetic System 1984", "EPSG", 6326L, string.Empty, "EPSG's WGS 84 datum has been the then current realisation. No distinction is made between the original WGS 84 frame, WGS 84 (G730), WGS 84 (G873) and WGS 84 (G1150). Since 1997, WGS 84 has been maintained within 10cm of the then current ITRF.", string.Empty);

	public static HorizontalDatum WGS72
	{
		get
		{
			HorizontalDatum horizontalDatum = new HorizontalDatum(ProjNet.CoordinateSystems.Ellipsoid.WGS72, null, DatumType.HD_Geocentric, "World Geodetic System 1972", "EPSG", 6322L, string.Empty, "Used by GPS before 1987. For Transit satellite positioning see also WGS 72BE. Datum code 6323 reserved for southern hemisphere ProjCS's.", string.Empty);
			horizontalDatum.Wgs84Parameters = new Wgs84ConversionInfo(0.0, 0.0, 4.5, 0.0, 0.0, 0.554, 0.219);
			return horizontalDatum;
		}
	}

	public static HorizontalDatum ETRF89
	{
		get
		{
			HorizontalDatum horizontalDatum = new HorizontalDatum(ProjNet.CoordinateSystems.Ellipsoid.GRS80, null, DatumType.HD_Geocentric, "European Terrestrial Reference System 1989", "EPSG", 6258L, "ETRF89", "The distinction in usage between ETRF89 and ETRS89 is confused: although in principle conceptually different in practice both are used for the realisation.", string.Empty);
			horizontalDatum.Wgs84Parameters = new Wgs84ConversionInfo();
			return horizontalDatum;
		}
	}

	public static HorizontalDatum ED50 => new HorizontalDatum(ProjNet.CoordinateSystems.Ellipsoid.International1924, new Wgs84ConversionInfo(-87.0, -98.0, -121.0, 0.0, 0.0, 0.0, 0.0), DatumType.HD_Geocentric, "European Datum 1950", "EPSG", 6230L, "ED50", string.Empty, string.Empty);

	public IEllipsoid Ellipsoid
	{
		get
		{
			return _Ellipsoid;
		}
		set
		{
			_Ellipsoid = value;
		}
	}

	public Wgs84ConversionInfo Wgs84Parameters
	{
		get
		{
			return _Wgs84ConversionInfo;
		}
		set
		{
			_Wgs84ConversionInfo = value;
		}
	}

	public override string WKT
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("DATUM[\"{0}\", {1}", base.Name, _Ellipsoid.WKT);
			if (_Wgs84ConversionInfo != null)
			{
				stringBuilder.AppendFormat(", {0}", _Wgs84ConversionInfo.WKT);
			}
			if (!string.IsNullOrEmpty(base.Authority) && base.AuthorityCode > 0)
			{
				stringBuilder.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", base.Authority, base.AuthorityCode);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}

	public override string XML => string.Format(CultureInfo.InvariantCulture.NumberFormat, "<CS_HorizontalDatum DatumType=\"{0}\">{1}{2}{3}</CS_HorizontalDatum>", (int)base.DatumType, base.InfoXml, Ellipsoid.XML, (Wgs84Parameters == null) ? string.Empty : Wgs84Parameters.XML);

	internal HorizontalDatum(IEllipsoid ellipsoid, Wgs84ConversionInfo toWgs84, DatumType type, string name, string authority, long code, string alias, string remarks, string abbreviation)
		: base(type, name, authority, code, alias, remarks, abbreviation)
	{
		_Ellipsoid = ellipsoid;
		_Wgs84ConversionInfo = toWgs84;
	}

	public override bool EqualParams(object obj)
	{
		if (!(obj is HorizontalDatum))
		{
			return false;
		}
		HorizontalDatum horizontalDatum = obj as HorizontalDatum;
		if (horizontalDatum.Wgs84Parameters == null && Wgs84Parameters != null)
		{
			return false;
		}
		if (horizontalDatum.Wgs84Parameters != null && !horizontalDatum.Wgs84Parameters.Equals(Wgs84Parameters))
		{
			return false;
		}
		if ((horizontalDatum != null && Ellipsoid != null && horizontalDatum.Ellipsoid.EqualParams(Ellipsoid)) || (horizontalDatum == null && Ellipsoid == null))
		{
			return base.DatumType == horizontalDatum.DatumType;
		}
		return false;
	}
}
