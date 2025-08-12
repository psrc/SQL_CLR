using System;
using System.Collections.Generic;

namespace ProjNet.CoordinateSystems.Transformations;

internal class GeocentricTransform : MathTransform
{
	private const double COS_67P5 = 0.3826834323650898;

	private const double AD_C = 1.0026;

	protected bool _isInverse;

	private double es;

	private double semiMajor;

	private double semiMinor;

	private double ab;

	private double ba;

	private double ses;

	protected List<ProjectionParameter> _Parameters;

	protected MathTransform _inverse;

	public override string WKT
	{
		get
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}
	}

	public override string XML
	{
		get
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}
	}

	public GeocentricTransform(List<ProjectionParameter> parameters, bool isInverse)
		: this(parameters)
	{
		_isInverse = isInverse;
	}

	internal GeocentricTransform(List<ProjectionParameter> parameters)
	{
		_Parameters = parameters;
		List<ProjectionParameter> parameters2 = _Parameters;
		Predicate<ProjectionParameter> match = delegate(ProjectionParameter par)
		{
			_Parameters = _Parameters;
			return par.Name.Equals("semi_major", StringComparison.OrdinalIgnoreCase);
		};
		semiMajor = parameters2.Find(match).Value;
		semiMinor = _Parameters.Find(delegate(ProjectionParameter par)
		{
			_Parameters = _Parameters;
			return par.Name.Equals("semi_minor", StringComparison.OrdinalIgnoreCase);
		}).Value;
		es = 1.0 - semiMinor * semiMinor / (semiMajor * semiMajor);
		ses = (Math.Pow(semiMajor, 2.0) - Math.Pow(semiMinor, 2.0)) / Math.Pow(semiMinor, 2.0);
		ba = semiMinor / semiMajor;
		ab = semiMajor / semiMinor;
	}

	public override IMathTransform Inverse()
	{
		if (_inverse == null)
		{
			_inverse = new GeocentricTransform(_Parameters, !_isInverse);
		}
		return _inverse;
	}

	private double[] DegreesToMeters(double[] lonlat)
	{
		double num = MathTransform.Degrees2Radians(lonlat[0]);
		double num2 = MathTransform.Degrees2Radians(lonlat[1]);
		double num3 = ((lonlat.Length < 3) ? 0.0 : (lonlat[2].Equals(double.NaN) ? 0.0 : lonlat[2]));
		double num4 = semiMajor / Math.Sqrt(1.0 - es * Math.Pow(Math.Sin(num2), 2.0));
		double num5 = (num4 + num3) * Math.Cos(num2) * Math.Cos(num);
		double num6 = (num4 + num3) * Math.Cos(num2) * Math.Sin(num);
		double num7 = ((1.0 - es) * num4 + num3) * Math.Sin(num2);
		return new double[3] { num5, num6, num7 };
	}

	private double[] MetersToDegrees(double[] pnt)
	{
		bool flag = false;
		double num = ((pnt.Length < 3) ? 0.0 : (pnt[2].Equals(double.NaN) ? 0.0 : pnt[2]));
		double num2 = 0.0;
		double rad = 0.0;
		double num3 = 0.0;
		if (pnt[0] != 0.0)
		{
			num2 = Math.Atan2(pnt[1], pnt[0]);
		}
		else if (pnt[1] > 0.0)
		{
			num2 = Math.PI / 2.0;
		}
		else if (pnt[1] < 0.0)
		{
			num2 = -Math.PI / 2.0;
		}
		else
		{
			flag = true;
			num2 = 0.0;
			if (num > 0.0)
			{
				rad = Math.PI / 2.0;
			}
			else
			{
				if (!(num < 0.0))
				{
					return new double[3]
					{
						MathTransform.Radians2Degrees(num2),
						MathTransform.Radians2Degrees(Math.PI / 2.0),
						0.0 - semiMinor
					};
				}
				rad = -Math.PI / 2.0;
			}
		}
		double num4 = pnt[0] * pnt[0] + pnt[1] * pnt[1];
		double num5 = Math.Sqrt(num4);
		double num6 = num * 1.0026;
		double num7 = Math.Sqrt(num6 * num6 + num4);
		double x = num6 / num7;
		double num8 = num5 / num7;
		double num9 = Math.Pow(x, 3.0);
		double num10 = num + semiMinor * ses * num9;
		double num11 = num5 - semiMajor * es * num8 * num8 * num8;
		double num12 = Math.Sqrt(num10 * num10 + num11 * num11);
		double num13 = num10 / num12;
		double num14 = num11 / num12;
		double num15 = semiMajor / Math.Sqrt(1.0 - es * num13 * num13);
		num3 = ((num14 >= 0.3826834323650898) ? (num5 / num14 - num15) : ((!(num14 <= -0.3826834323650898)) ? (num / num13 + num15 * (es - 1.0)) : (num5 / (0.0 - num14) - num15)));
		if (!flag)
		{
			rad = Math.Atan(num13 / num14);
		}
		return new double[3]
		{
			MathTransform.Radians2Degrees(num2),
			MathTransform.Radians2Degrees(rad),
			num3
		};
	}

	public override double[] Transform(double[] point)
	{
		if (!_isInverse)
		{
			return DegreesToMeters(point);
		}
		return MetersToDegrees(point);
	}

	public override List<double[]> TransformList(List<double[]> points)
	{
		List<double[]> list = new List<double[]>(points.Count);
		for (int i = 0; i < points.Count; i++)
		{
			double[] point = points[i];
			list.Add(Transform(point));
		}
		return list;
	}

	public override void Invert()
	{
		_isInverse = !_isInverse;
	}
}
