namespace ProjNet.CoordinateSystems;

public interface ICoordinateSystemAuthorityFactory
{
	string Authority { get; }

	string DescriptionText { get; }

	IProjectedCoordinateSystem CreateProjectedCoordinateSystem(long code);

	IGeographicCoordinateSystem CreateGeographicCoordinateSystem(long code);

	IHorizontalDatum CreateHorizontalDatum(long code);

	IEllipsoid CreateEllipsoid(long code);

	IPrimeMeridian CreatePrimeMeridian(long code);

	ILinearUnit CreateLinearUnit(long code);

	IAngularUnit CreateAngularUnit(long code);

	IVerticalDatum CreateVerticalDatum(long code);

	IVerticalCoordinateSystem CreateVerticalCoordinateSystem(long code);

	ICompoundCoordinateSystem CreateCompoundCoordinateSystem(long code);

	IHorizontalCoordinateSystem CreateHorizontalCoordinateSystem(long code);

	string GeoidFromWktName(string wkt);

	string WktGeoidName(string geoid);
}
