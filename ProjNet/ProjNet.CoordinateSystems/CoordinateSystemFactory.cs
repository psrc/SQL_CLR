using System;
using System.Collections.Generic;
using ProjNet.Converters.WellKnownText;

namespace ProjNet.CoordinateSystems;

public class CoordinateSystemFactory : ICoordinateSystemFactory
{
	public ICoordinateSystem CreateFromXml(string xml)
	{
		throw new NotImplementedException();
	}

	public ICoordinateSystem CreateFromWkt(string WKT)
	{
		return CoordinateSystemWktReader.Parse(WKT) as ICoordinateSystem;
	}

	public ICompoundCoordinateSystem CreateCompoundCoordinateSystem(string name, ICoordinateSystem head, ICoordinateSystem tail)
	{
		throw new NotImplementedException();
	}

	public IFittedCoordinateSystem CreateFittedCoordinateSystem(string name, ICoordinateSystem baseCoordinateSystem, string toBaseWkt, List<AxisInfo> arAxes)
	{
		throw new NotImplementedException();
	}

	public ILocalCoordinateSystem CreateLocalCoordinateSystem(string name, ILocalDatum datum, IUnit unit, List<AxisInfo> axes)
	{
		throw new NotImplementedException();
	}

	public IEllipsoid CreateEllipsoid(string name, double semiMajorAxis, double semiMinorAxis, ILinearUnit linearUnit)
	{
		double inverseFlattening = 0.0;
		if (semiMajorAxis != semiMinorAxis)
		{
			inverseFlattening = semiMajorAxis / (semiMajorAxis - semiMinorAxis);
		}
		return new Ellipsoid(semiMajorAxis, semiMinorAxis, inverseFlattening, isIvfDefinitive: false, linearUnit, name, string.Empty, -1L, string.Empty, string.Empty, string.Empty);
	}

	public IEllipsoid CreateFlattenedSphere(string name, double semiMajorAxis, double inverseFlattening, ILinearUnit linearUnit)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Invalid name");
		}
		return new Ellipsoid(semiMajorAxis, -1.0, inverseFlattening, isIvfDefinitive: true, linearUnit, name, string.Empty, -1L, string.Empty, string.Empty, string.Empty);
	}

	public IProjectedCoordinateSystem CreateProjectedCoordinateSystem(string name, IGeographicCoordinateSystem gcs, IProjection projection, ILinearUnit linearUnit, AxisInfo axis0, AxisInfo axis1)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Invalid name");
		}
		if (gcs == null)
		{
			throw new ArgumentException("Geographic coordinate system was null");
		}
		if (projection == null)
		{
			throw new ArgumentException("Projection was null");
		}
		if (linearUnit == null)
		{
			throw new ArgumentException("Linear unit was null");
		}
		List<AxisInfo> list = new List<AxisInfo>(2);
		list.Add(axis0);
		list.Add(axis1);
		return new ProjectedCoordinateSystem(null, gcs, linearUnit, projection, list, name, string.Empty, -1L, string.Empty, string.Empty, string.Empty);
	}

	public IProjection CreateProjection(string name, string wktProjectionClass, List<ProjectionParameter> parameters)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Invalid name");
		}
		if (parameters == null || parameters.Count == 0)
		{
			throw new ArgumentException("Invalid projection parameters");
		}
		return new Projection(wktProjectionClass, parameters, name, string.Empty, -1L, string.Empty, string.Empty, string.Empty);
	}

	public IHorizontalDatum CreateHorizontalDatum(string name, DatumType datumType, IEllipsoid ellipsoid, Wgs84ConversionInfo toWgs84)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Invalid name");
		}
		if (ellipsoid == null)
		{
			throw new ArgumentException("Ellipsoid was null");
		}
		return new HorizontalDatum(ellipsoid, toWgs84, datumType, name, string.Empty, -1L, string.Empty, string.Empty, string.Empty);
	}

	public IPrimeMeridian CreatePrimeMeridian(string name, IAngularUnit angularUnit, double longitude)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Invalid name");
		}
		return new PrimeMeridian(longitude, angularUnit, name, string.Empty, -1L, string.Empty, string.Empty, string.Empty);
	}

	public IGeographicCoordinateSystem CreateGeographicCoordinateSystem(string name, IAngularUnit angularUnit, IHorizontalDatum datum, IPrimeMeridian primeMeridian, AxisInfo axis0, AxisInfo axis1)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Invalid name");
		}
		List<AxisInfo> list = new List<AxisInfo>(2);
		list.Add(axis0);
		list.Add(axis1);
		return new GeographicCoordinateSystem(angularUnit, datum, primeMeridian, list, name, string.Empty, -1L, string.Empty, string.Empty, string.Empty);
	}

	public ILocalDatum CreateLocalDatum(string name, DatumType datumType)
	{
		throw new NotImplementedException();
	}

	public IVerticalDatum CreateVerticalDatum(string name, DatumType datumType)
	{
		throw new NotImplementedException();
	}

	public IVerticalCoordinateSystem CreateVerticalCoordinateSystem(string name, IVerticalDatum datum, ILinearUnit verticalUnit, AxisInfo axis)
	{
		throw new NotImplementedException();
	}

	public IGeocentricCoordinateSystem CreateGeocentricCoordinateSystem(string name, IHorizontalDatum datum, ILinearUnit linearUnit, IPrimeMeridian primeMeridian)
	{
		if (string.IsNullOrEmpty(name))
		{
			throw new ArgumentException("Invalid name");
		}
		List<AxisInfo> list = new List<AxisInfo>(3);
		list.Add(new AxisInfo("X", AxisOrientationEnum.Other));
		list.Add(new AxisInfo("Y", AxisOrientationEnum.Other));
		list.Add(new AxisInfo("Z", AxisOrientationEnum.Other));
		return new GeocentricCoordinateSystem(datum, linearUnit, primeMeridian, list, name, string.Empty, -1L, string.Empty, string.Empty, string.Empty);
	}
}
