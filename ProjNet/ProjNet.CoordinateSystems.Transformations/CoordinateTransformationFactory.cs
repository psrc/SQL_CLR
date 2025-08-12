using System;
using System.Collections.Generic;
using System.Globalization;
using ProjNet.CoordinateSystems.Projections;

namespace ProjNet.CoordinateSystems.Transformations;

public class CoordinateTransformationFactory : ICoordinateTransformationFactory
{
	public ICoordinateTransformation CreateFromCoordinateSystems(ICoordinateSystem sourceCS, ICoordinateSystem targetCS)
	{
		if (sourceCS is IProjectedCoordinateSystem && targetCS is IGeographicCoordinateSystem)
		{
			return Proj2Geog((IProjectedCoordinateSystem)sourceCS, (IGeographicCoordinateSystem)targetCS);
		}
		if (sourceCS is IGeographicCoordinateSystem && targetCS is IProjectedCoordinateSystem)
		{
			return Geog2Proj((IGeographicCoordinateSystem)sourceCS, (IProjectedCoordinateSystem)targetCS);
		}
		if (sourceCS is IGeographicCoordinateSystem && targetCS is IGeocentricCoordinateSystem)
		{
			return Geog2Geoc((IGeographicCoordinateSystem)sourceCS, (IGeocentricCoordinateSystem)targetCS);
		}
		if (sourceCS is IGeocentricCoordinateSystem && targetCS is IGeographicCoordinateSystem)
		{
			return Geoc2Geog((IGeocentricCoordinateSystem)sourceCS, (IGeographicCoordinateSystem)targetCS);
		}
		if (sourceCS is IProjectedCoordinateSystem && targetCS is IProjectedCoordinateSystem)
		{
			return Proj2Proj(sourceCS as IProjectedCoordinateSystem, targetCS as IProjectedCoordinateSystem);
		}
		if (sourceCS is IGeocentricCoordinateSystem && targetCS is IGeocentricCoordinateSystem)
		{
			return CreateGeoc2Geoc((IGeocentricCoordinateSystem)sourceCS, (IGeocentricCoordinateSystem)targetCS);
		}
		if (sourceCS is IGeographicCoordinateSystem && targetCS is IGeographicCoordinateSystem)
		{
			return CreateGeog2Geog(sourceCS as IGeographicCoordinateSystem, targetCS as IGeographicCoordinateSystem);
		}
		throw new NotSupportedException("No support for transforming between the two specified coordinate systems");
	}

	private static void SimplifyTrans(ConcatenatedTransform mtrans, ref List<ICoordinateTransformation> MTs)
	{
		foreach (ICoordinateTransformation coordinateTransformation in mtrans.CoordinateTransformationList)
		{
			if (coordinateTransformation is ConcatenatedTransform)
			{
				SimplifyTrans(coordinateTransformation as ConcatenatedTransform, ref MTs);
			}
			else
			{
				MTs.Add(coordinateTransformation);
			}
		}
	}

	private static ICoordinateTransformation Geog2Geoc(IGeographicCoordinateSystem source, IGeocentricCoordinateSystem target)
	{
		IMathTransform mathTransform = CreateCoordinateOperation(target);
		return new CoordinateTransformation(source, target, TransformType.Conversion, mathTransform, string.Empty, string.Empty, -1L, string.Empty, string.Empty);
	}

	private static ICoordinateTransformation Geoc2Geog(IGeocentricCoordinateSystem source, IGeographicCoordinateSystem target)
	{
		IMathTransform mathTransform = CreateCoordinateOperation(source).Inverse();
		return new CoordinateTransformation(source, target, TransformType.Conversion, mathTransform, string.Empty, string.Empty, -1L, string.Empty, string.Empty);
	}

	private static ICoordinateTransformation Proj2Proj(IProjectedCoordinateSystem source, IProjectedCoordinateSystem target)
	{
		ConcatenatedTransform concatenatedTransform = new ConcatenatedTransform();
		CoordinateTransformationFactory coordinateTransformationFactory = new CoordinateTransformationFactory();
		concatenatedTransform.CoordinateTransformationList.Add(coordinateTransformationFactory.CreateFromCoordinateSystems(source, source.GeographicCoordinateSystem));
		concatenatedTransform.CoordinateTransformationList.Add(coordinateTransformationFactory.CreateFromCoordinateSystems(source.GeographicCoordinateSystem, target.GeographicCoordinateSystem));
		concatenatedTransform.CoordinateTransformationList.Add(coordinateTransformationFactory.CreateFromCoordinateSystems(target.GeographicCoordinateSystem, target));
		return new CoordinateTransformation(source, target, TransformType.Transformation, concatenatedTransform, string.Empty, string.Empty, -1L, string.Empty, string.Empty);
	}

	private static ICoordinateTransformation Geog2Proj(IGeographicCoordinateSystem source, IProjectedCoordinateSystem target)
	{
		if (source.EqualParams(target.GeographicCoordinateSystem))
		{
			IMathTransform mathTransform = CreateCoordinateOperation(target.Projection, target.GeographicCoordinateSystem.HorizontalDatum.Ellipsoid, target.LinearUnit);
			return new CoordinateTransformation(source, target, TransformType.Transformation, mathTransform, string.Empty, string.Empty, -1L, string.Empty, string.Empty);
		}
		ConcatenatedTransform concatenatedTransform = new ConcatenatedTransform();
		CoordinateTransformationFactory coordinateTransformationFactory = new CoordinateTransformationFactory();
		concatenatedTransform.CoordinateTransformationList.Add(coordinateTransformationFactory.CreateFromCoordinateSystems(source, target.GeographicCoordinateSystem));
		concatenatedTransform.CoordinateTransformationList.Add(coordinateTransformationFactory.CreateFromCoordinateSystems(target.GeographicCoordinateSystem, target));
		return new CoordinateTransformation(source, target, TransformType.Transformation, concatenatedTransform, string.Empty, string.Empty, -1L, string.Empty, string.Empty);
	}

	private static ICoordinateTransformation Proj2Geog(IProjectedCoordinateSystem source, IGeographicCoordinateSystem target)
	{
		if (source.GeographicCoordinateSystem.EqualParams(target))
		{
			IMathTransform mathTransform = CreateCoordinateOperation(source.Projection, source.GeographicCoordinateSystem.HorizontalDatum.Ellipsoid, source.LinearUnit).Inverse();
			return new CoordinateTransformation(source, target, TransformType.Transformation, mathTransform, string.Empty, string.Empty, -1L, string.Empty, string.Empty);
		}
		ConcatenatedTransform concatenatedTransform = new ConcatenatedTransform();
		CoordinateTransformationFactory coordinateTransformationFactory = new CoordinateTransformationFactory();
		concatenatedTransform.CoordinateTransformationList.Add(coordinateTransformationFactory.CreateFromCoordinateSystems(source, source.GeographicCoordinateSystem));
		concatenatedTransform.CoordinateTransformationList.Add(coordinateTransformationFactory.CreateFromCoordinateSystems(source.GeographicCoordinateSystem, target));
		return new CoordinateTransformation(source, target, TransformType.Transformation, concatenatedTransform, string.Empty, string.Empty, -1L, string.Empty, string.Empty);
	}

	private ICoordinateTransformation CreateGeog2Geog(IGeographicCoordinateSystem source, IGeographicCoordinateSystem target)
	{
		if (source.HorizontalDatum.EqualParams(target.HorizontalDatum))
		{
			return new CoordinateTransformation(source, target, TransformType.Conversion, new GeographicTransform(source, target), string.Empty, string.Empty, -1L, string.Empty, string.Empty);
		}
		CoordinateTransformationFactory coordinateTransformationFactory = new CoordinateTransformationFactory();
		CoordinateSystemFactory coordinateSystemFactory = new CoordinateSystemFactory();
		IGeocentricCoordinateSystem geocentricCoordinateSystem = coordinateSystemFactory.CreateGeocentricCoordinateSystem(source.HorizontalDatum.Name + " Geocentric", source.HorizontalDatum, LinearUnit.Metre, source.PrimeMeridian);
		IGeocentricCoordinateSystem geocentricCoordinateSystem2 = coordinateSystemFactory.CreateGeocentricCoordinateSystem(target.HorizontalDatum.Name + " Geocentric", target.HorizontalDatum, LinearUnit.Metre, source.PrimeMeridian);
		ConcatenatedTransform concatenatedTransform = new ConcatenatedTransform();
		concatenatedTransform.CoordinateTransformationList.Add(coordinateTransformationFactory.CreateFromCoordinateSystems(source, geocentricCoordinateSystem));
		concatenatedTransform.CoordinateTransformationList.Add(coordinateTransformationFactory.CreateFromCoordinateSystems(geocentricCoordinateSystem, geocentricCoordinateSystem2));
		concatenatedTransform.CoordinateTransformationList.Add(coordinateTransformationFactory.CreateFromCoordinateSystems(geocentricCoordinateSystem2, target));
		return new CoordinateTransformation(source, target, TransformType.Transformation, concatenatedTransform, string.Empty, string.Empty, -1L, string.Empty, string.Empty);
	}

	private static ICoordinateTransformation CreateGeoc2Geoc(IGeocentricCoordinateSystem source, IGeocentricCoordinateSystem target)
	{
		ConcatenatedTransform concatenatedTransform = new ConcatenatedTransform();
		if (source.HorizontalDatum.Wgs84Parameters != null && !source.HorizontalDatum.Wgs84Parameters.HasZeroValuesOnly)
		{
			concatenatedTransform.CoordinateTransformationList.Add(new CoordinateTransformation((target.HorizontalDatum.Wgs84Parameters == null || target.HorizontalDatum.Wgs84Parameters.HasZeroValuesOnly) ? target : GeocentricCoordinateSystem.WGS84, source, TransformType.Transformation, new DatumTransform(source.HorizontalDatum.Wgs84Parameters), "", "", -1L, "", ""));
		}
		if (target.HorizontalDatum.Wgs84Parameters != null && !target.HorizontalDatum.Wgs84Parameters.HasZeroValuesOnly)
		{
			concatenatedTransform.CoordinateTransformationList.Add(new CoordinateTransformation((source.HorizontalDatum.Wgs84Parameters == null || source.HorizontalDatum.Wgs84Parameters.HasZeroValuesOnly) ? source : GeocentricCoordinateSystem.WGS84, target, TransformType.Transformation, new DatumTransform(target.HorizontalDatum.Wgs84Parameters).Inverse(), "", "", -1L, "", ""));
		}
		if (concatenatedTransform.CoordinateTransformationList.Count == 1)
		{
			return new CoordinateTransformation(source, target, TransformType.ConversionAndTransformation, concatenatedTransform.CoordinateTransformationList[0].MathTransform, "", "", -1L, "", "");
		}
		return new CoordinateTransformation(source, target, TransformType.ConversionAndTransformation, concatenatedTransform, "", "", -1L, "", "");
	}

	private static IMathTransform CreateCoordinateOperation(IGeocentricCoordinateSystem geo)
	{
		List<ProjectionParameter> list = new List<ProjectionParameter>(2);
		list.Add(new ProjectionParameter("semi_major", geo.HorizontalDatum.Ellipsoid.SemiMajorAxis));
		list.Add(new ProjectionParameter("semi_minor", geo.HorizontalDatum.Ellipsoid.SemiMinorAxis));
		return new GeocentricTransform(list);
	}

	private static IMathTransform CreateCoordinateOperation(IProjection projection, IEllipsoid ellipsoid, ILinearUnit unit)
	{
		List<ProjectionParameter> list = new List<ProjectionParameter>(projection.NumParameters);
		for (int i = 0; i < projection.NumParameters; i++)
		{
			list.Add(projection.GetParameter(i));
		}
		list.Add(new ProjectionParameter("semi_major", ellipsoid.SemiMajorAxis));
		list.Add(new ProjectionParameter("semi_minor", ellipsoid.SemiMinorAxis));
		list.Add(new ProjectionParameter("unit", unit.MetersPerUnit));
		IMathTransform mathTransform = null;
		switch (projection.ClassName.ToLower(CultureInfo.InvariantCulture).Replace(' ', '_'))
		{
		case "mercator":
		case "mercator_1sp":
		case "mercator_2sp":
			return new Mercator(list);
		case "transverse_mercator":
			return new TransverseMercator(list);
		case "albers":
		case "albers_conic_equal_area":
			return new AlbersProjection(list);
		case "krovak":
			return new KrovakProjection(list);
		case "lambert_conformal_conic":
		case "lambert_conformal_conic_2sp":
		case "lambert_conic_conformal_(2sp)":
			return new LambertConformalConic2SP(list);
		default:
			throw new NotSupportedException($"Projection {projection.ClassName} is not supported.");
		}
	}
}
