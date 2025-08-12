namespace ProjNet.CoordinateSystems;

public interface IHorizontalDatum : IDatum, IInfo
{
	IEllipsoid Ellipsoid { get; set; }

	Wgs84ConversionInfo Wgs84Parameters { get; set; }
}
