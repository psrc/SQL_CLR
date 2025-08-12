using System;
using System.Collections.Generic;
using ProjNet.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections;

internal class AlbersProjection : MapProjection
{
	private double _falseEasting;

	private double _falseNorthing;

	private double C;

	private double e;

	private double e_sq;

	private double ro0;

	private double n;

	private double lon_center;

	public AlbersProjection(List<ProjectionParameter> parameters)
		: this(parameters, isInverse: false)
	{
	}

	public AlbersProjection(List<ProjectionParameter> parameters, bool isInverse)
		: base(parameters, isInverse)
	{
		base.Name = "Albers_Conic_Equal_Area";
		ProjectionParameter parameter = GetParameter("longitude_of_center");
		ProjectionParameter parameter2 = GetParameter("latitude_of_center");
		ProjectionParameter parameter3 = GetParameter("standard_parallel_1");
		ProjectionParameter parameter4 = GetParameter("standard_parallel_2");
		ProjectionParameter parameter5 = GetParameter("false_easting");
		ProjectionParameter parameter6 = GetParameter("false_northing");
		if (parameter == null)
		{
			parameter = GetParameter("central_meridian");
			if (parameter == null)
			{
				throw new ArgumentException("Missing projection parameter 'longitude_of_center'");
			}
		}
		if (parameter2 == null)
		{
			parameter2 = GetParameter("latitude_of_origin");
			if (parameter2 == null)
			{
				throw new ArgumentException("Missing projection parameter 'latitude_of_center'");
			}
		}
		if (parameter3 == null)
		{
			throw new ArgumentException("Missing projection parameter 'standard_parallel_1'");
		}
		if (parameter4 == null)
		{
			throw new ArgumentException("Missing projection parameter 'standard_parallel_2'");
		}
		if (parameter5 == null)
		{
			throw new ArgumentException("Missing projection parameter 'false_easting'");
		}
		if (parameter6 == null)
		{
			throw new ArgumentException("Missing projection parameter 'false_northing'");
		}
		lon_center = MathTransform.Degrees2Radians(parameter.Value);
		double lat = MathTransform.Degrees2Radians(parameter2.Value);
		double num = MathTransform.Degrees2Radians(parameter3.Value);
		double num2 = MathTransform.Degrees2Radians(parameter4.Value);
		_falseEasting = parameter5.Value * _metersPerUnit;
		_falseNorthing = parameter6.Value * _metersPerUnit;
		if (Math.Abs(num + num2) < double.Epsilon)
		{
			throw new ArgumentException("Equal latitudes for standard parallels on opposite sides of Equator.");
		}
		e_sq = 1.0 - Math.Pow(_semiMinor / _semiMajor, 2.0);
		e = Math.Sqrt(e_sq);
		double num3 = alpha(num);
		double num4 = alpha(num2);
		double x = Math.Cos(num) / Math.Sqrt(1.0 - e_sq * Math.Pow(Math.Sin(num), 2.0));
		double x2 = Math.Cos(num2) / Math.Sqrt(1.0 - e_sq * Math.Pow(Math.Sin(num2), 2.0));
		n = (Math.Pow(x, 2.0) - Math.Pow(x2, 2.0)) / (num4 - num3);
		C = Math.Pow(x, 2.0) + n * num3;
		ro0 = Ro(alpha(lat));
	}

	public override double[] DegreesToMeters(double[] lonlat)
	{
		double num = MathTransform.Degrees2Radians(lonlat[0]);
		double lat = MathTransform.Degrees2Radians(lonlat[1]);
		double a = alpha(lat);
		double num2 = Ro(a);
		double num3 = n * (num - lon_center);
		num = _falseEasting + num2 * Math.Sin(num3);
		lat = _falseNorthing + ro0 - num2 * Math.Cos(num3);
		if (lonlat.Length == 2)
		{
			return new double[2]
			{
				num / _metersPerUnit,
				lat / _metersPerUnit
			};
		}
		return new double[3]
		{
			num / _metersPerUnit,
			lat / _metersPerUnit,
			lonlat[2]
		};
	}

	public override double[] MetersToDegrees(double[] p)
	{
		double num = Math.Atan((p[0] * _metersPerUnit - _falseEasting) / (ro0 - (p[1] * _metersPerUnit - _falseNorthing)));
		double x = Math.Sqrt(Math.Pow(p[0] * _metersPerUnit - _falseEasting, 2.0) + Math.Pow(ro0 - (p[1] * _metersPerUnit - _falseNorthing), 2.0));
		double num2 = (C - Math.Pow(x, 2.0) * Math.Pow(n, 2.0) / Math.Pow(_semiMajor, 2.0)) / n;
		Math.Sin(num2 / (1.0 - (1.0 - e_sq) / (2.0 * e) * Math.Log((1.0 - e) / (1.0 + e))));
		double num3 = Math.Asin(num2 * 0.5);
		double num4 = double.MaxValue;
		int num5 = 0;
		while (Math.Abs(num3 - num4) > 1E-06)
		{
			num4 = num3;
			double num6 = Math.Sin(num3);
			double num7 = e_sq * Math.Pow(num6, 2.0);
			num3 += Math.Pow(1.0 - num7, 2.0) / (2.0 * Math.Cos(num3)) * (num2 / (1.0 - e_sq) - num6 / (1.0 - num7) + 1.0 / (2.0 * e) * Math.Log((1.0 - e * num6) / (1.0 + e * num6)));
			num5++;
			if (num5 > 25)
			{
				throw new ArgumentException("Transformation failed to converge in Albers backwards transformation");
			}
		}
		double rad = lon_center + num / n;
		if (p.Length == 2)
		{
			return new double[2]
			{
				MathTransform.Radians2Degrees(rad),
				MathTransform.Radians2Degrees(num3)
			};
		}
		return new double[3]
		{
			MathTransform.Radians2Degrees(rad),
			MathTransform.Radians2Degrees(num3),
			p[2]
		};
	}

	public override IMathTransform Inverse()
	{
		if (_inverse == null)
		{
			_inverse = new AlbersProjection(_Parameters, !_isInverse);
		}
		return _inverse;
	}

	private double alpha(double lat)
	{
		double num = Math.Sin(lat);
		double num2 = Math.Pow(num, 2.0);
		return (1.0 - e_sq) * (num / (1.0 - e_sq * num2) - 1.0 / (2.0 * e) * Math.Log((1.0 - e * num) / (1.0 + e * num)));
	}

	private double Ro(double a)
	{
		return _semiMajor * Math.Sqrt(C - n * a) / n;
	}
}
