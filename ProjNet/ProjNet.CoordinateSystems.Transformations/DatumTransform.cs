using System;
using System.Collections.Generic;

namespace ProjNet.CoordinateSystems.Transformations;

internal class DatumTransform : MathTransform
{
	protected IMathTransform _inverse;

	private Wgs84ConversionInfo _ToWgs94;

	private double[] v;

	private bool _isInverse;

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

	public DatumTransform(Wgs84ConversionInfo towgs84)
		: this(towgs84, isInverse: false)
	{
	}

	private DatumTransform(Wgs84ConversionInfo towgs84, bool isInverse)
	{
		_ToWgs94 = towgs84;
		v = _ToWgs94.GetAffineTransform();
		_isInverse = isInverse;
	}

	public override IMathTransform Inverse()
	{
		if (_inverse == null)
		{
			_inverse = new DatumTransform(_ToWgs94, !_isInverse);
		}
		return _inverse;
	}

	private double[] Apply(double[] p)
	{
		return new double[3]
		{
			v[0] * p[0] - v[3] * p[1] + v[2] * p[2] + v[4],
			v[3] * p[0] + v[0] * p[1] - v[1] * p[2] + v[5],
			(0.0 - v[2]) * p[0] + v[1] * p[1] + v[0] * p[2] + v[6]
		};
	}

	private double[] ApplyInverted(double[] p)
	{
		return new double[3]
		{
			v[0] * p[0] + v[3] * p[1] - v[2] * p[2] - v[4],
			(0.0 - v[3]) * p[0] + v[0] * p[1] + v[1] * p[2] - v[5],
			v[2] * p[0] - v[1] * p[1] + v[0] * p[2] - v[6]
		};
	}

	public override double[] Transform(double[] point)
	{
		if (!_isInverse)
		{
			return Apply(point);
		}
		return ApplyInverted(point);
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
		_isInverse = !_isInverse;
	}
}
