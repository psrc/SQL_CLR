using System.Collections.Generic;

namespace ProjNet.CoordinateSystems;

public interface ICoordinateSystemFactory
{
	ICompoundCoordinateSystem CreateCompoundCoordinateSystem(string name, ICoordinateSystem head, ICoordinateSystem tail);

	IEllipsoid CreateEllipsoid(string name, double semiMajorAxis, double semiMinorAxis, ILinearUnit linearUnit);

	IFittedCoordinateSystem CreateFittedCoordinateSystem(string name, ICoordinateSystem baseCoordinateSystem, string toBaseWkt, List<AxisInfo> arAxes);

	IEllipsoid CreateFlattenedSphere(string name, double semiMajorAxis, double inverseFlattening, ILinearUnit linearUnit);

	ICoordinateSystem CreateFromXml(string xml);

	ICoordinateSystem CreateFromWkt(string WKT);

	IGeographicCoordinateSystem CreateGeographicCoordinateSystem(string name, IAngularUnit angularUnit, IHorizontalDatum datum, IPrimeMeridian primeMeridian, AxisInfo axis0, AxisInfo axis1);

	IHorizontalDatum CreateHorizontalDatum(string name, DatumType datumType, IEllipsoid ellipsoid, Wgs84ConversionInfo toWgs84);

	ILocalCoordinateSystem CreateLocalCoordinateSystem(string name, ILocalDatum datum, IUnit unit, List<AxisInfo> axes);

	ILocalDatum CreateLocalDatum(string name, DatumType datumType);

	IPrimeMeridian CreatePrimeMeridian(string name, IAngularUnit angularUnit, double longitude);

	IProjectedCoordinateSystem CreateProjectedCoordinateSystem(string name, IGeographicCoordinateSystem gcs, IProjection projection, ILinearUnit linearUnit, AxisInfo axis0, AxisInfo axis1);

	IProjection CreateProjection(string name, string wktProjectionClass, List<ProjectionParameter> Parameters);

	IVerticalCoordinateSystem CreateVerticalCoordinateSystem(string name, IVerticalDatum datum, ILinearUnit verticalUnit, AxisInfo axis);

	IVerticalDatum CreateVerticalDatum(string name, DatumType datumType);
}
