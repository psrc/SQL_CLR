using System;
using System.Collections.Generic;

namespace ProjNet.CoordinateSystems.Transformations;

public class GeographicTransform : MathTransform
{
	private IGeographicCoordinateSystem _SourceGCS;

	private IGeographicCoordinateSystem _TargetGCS;

	public IGeographicCoordinateSystem SourceGCS
	{
		get
		{
			return _SourceGCS;
		}
		set
		{
			_SourceGCS = value;
		}
	}

	public IGeographicCoordinateSystem TargetGCS
	{
		get
		{
			return _TargetGCS;
		}
		set
		{
			_TargetGCS = value;
		}
	}

	public override string WKT
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override string XML
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal GeographicTransform(IGeographicCoordinateSystem sourceGCS, IGeographicCoordinateSystem targetGCS)
	{
		_SourceGCS = sourceGCS;
		_TargetGCS = targetGCS;
	}

	public override IMathTransform Inverse()
	{
		throw new NotImplementedException();
	}

	public override double[] Transform(double[] point)
	{
		double[] array = (double[])point.Clone();
		array[0] /= SourceGCS.AngularUnit.RadiansPerUnit;
		array[0] -= SourceGCS.PrimeMeridian.Longitude / SourceGCS.PrimeMeridian.AngularUnit.RadiansPerUnit;
		array[0] += TargetGCS.PrimeMeridian.Longitude / TargetGCS.PrimeMeridian.AngularUnit.RadiansPerUnit;
		array[0] *= SourceGCS.AngularUnit.RadiansPerUnit;
		return array;
	}

	public override List<double[]> TransformList(List<double[]> points)
	{
		List<double[]> list = new List<double[]>(points.Count);
		foreach (double[] point in points)
		{
			list.Add(Transform(point));
		}
		return list;
	}

	public override void Invert()
	{
		throw new NotImplementedException();
	}
}
