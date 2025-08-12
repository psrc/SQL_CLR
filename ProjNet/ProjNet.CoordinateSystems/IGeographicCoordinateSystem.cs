namespace ProjNet.CoordinateSystems;

public interface IGeographicCoordinateSystem : IHorizontalCoordinateSystem, ICoordinateSystem, IInfo
{
	IAngularUnit AngularUnit { get; set; }

	IPrimeMeridian PrimeMeridian { get; set; }

	int NumConversionToWGS84 { get; }

	Wgs84ConversionInfo GetWgs84ConversionInfo(int index);
}
