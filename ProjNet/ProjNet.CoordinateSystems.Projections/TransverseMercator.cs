using System;
using System.Collections.Generic;
using ProjNet.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections;

internal class TransverseMercator : MapProjection
{
	private double scale_factor;

	private double central_meridian;

	private double lat_origin;

	private double e0;

	private double e1;

	private double e2;

	private double e3;

	private double e;

	private double es;

	private double esp;

	private double ml0;

	private double false_northing;

	private double false_easting;

	public TransverseMercator(List<ProjectionParameter> parameters)
		: this(parameters, inverse: false)
	{
	}

	public TransverseMercator(List<ProjectionParameter> parameters, bool inverse)
		: base(parameters, inverse)
	{
		base.Name = "Transverse_Mercator";
		base.Authority = "EPSG";
		base.AuthorityCode = 9807L;
		ProjectionParameter parameter = GetParameter("scale_factor");
		ProjectionParameter parameter2 = GetParameter("central_meridian");
		ProjectionParameter parameter3 = GetParameter("latitude_of_origin");
		ProjectionParameter parameter4 = GetParameter("false_easting");
		ProjectionParameter parameter5 = GetParameter("false_northing");
		if (parameter == null)
		{
			throw new ArgumentException("Missing projection parameter 'scale_factor'");
		}
		if (parameter2 == null)
		{
			throw new ArgumentException("Missing projection parameter 'central_meridian'");
		}
		if (parameter3 == null)
		{
			throw new ArgumentException("Missing projection parameter 'latitude_of_origin'");
		}
		if (parameter4 == null)
		{
			throw new ArgumentException("Missing projection parameter 'false_easting'");
		}
		if (parameter5 == null)
		{
			throw new ArgumentException("Missing projection parameter 'false_northing'");
		}
		scale_factor = parameter.Value;
		central_meridian = MathTransform.Degrees2Radians(parameter2.Value);
		lat_origin = MathTransform.Degrees2Radians(parameter3.Value);
		false_easting = parameter4.Value * _metersPerUnit;
		false_northing = parameter5.Value * _metersPerUnit;
		es = 1.0 - Math.Pow(_semiMinor / _semiMajor, 2.0);
		e = Math.Sqrt(es);
		e0 = MapProjection.e0fn(es);
		e1 = MapProjection.e1fn(es);
		e2 = MapProjection.e2fn(es);
		e3 = MapProjection.e3fn(es);
		ml0 = _semiMajor * MapProjection.mlfn(e0, e1, e2, e3, lat_origin);
		esp = es / (1.0 - es);
	}

	public override double[] DegreesToMeters(double[] lonlat)
	{
		double num = MathTransform.Degrees2Radians(lonlat[0]);
		double num2 = MathTransform.Degrees2Radians(lonlat[1]);
		double num3 = 0.0;
		num3 = MapProjection.adjust_lon(num - central_meridian);
		MapProjection.sincos(num2, out var sin_val, out var cos_val);
		double num4 = cos_val * num3;
		double num5 = Math.Pow(num4, 2.0);
		double num6 = esp * Math.Pow(cos_val, 2.0);
		double num7 = Math.Tan(num2);
		double num8 = Math.Pow(num7, 2.0);
		double d = 1.0 - es * Math.Pow(sin_val, 2.0);
		double num9 = _semiMajor / Math.Sqrt(d);
		double num10 = _semiMajor * MapProjection.mlfn(e0, e1, e2, e3, num2);
		double num11 = scale_factor * num9 * num4 * (1.0 + num5 / 6.0 * (1.0 - num8 + num6 + num5 / 20.0 * (5.0 - 18.0 * num8 + Math.Pow(num8, 2.0) + 72.0 * num6 - 58.0 * esp))) + false_easting;
		double num12 = scale_factor * (num10 - ml0 + num9 * num7 * (num5 * (0.5 + num5 / 24.0 * (5.0 - num8 + 9.0 * num6 + 4.0 * Math.Pow(num6, 2.0) + num5 / 30.0 * (61.0 - 58.0 * num8 + Math.Pow(num8, 2.0) + 600.0 * num6 - 330.0 * esp))))) + false_northing;
		if (lonlat.Length < 3)
		{
			return new double[2]
			{
				num11 / _metersPerUnit,
				num12 / _metersPerUnit
			};
		}
		return new double[3]
		{
			num11 / _metersPerUnit,
			num12 / _metersPerUnit,
			lonlat[2]
		};
	}

	public override double[] MetersToDegrees(double[] p)
	{
		long num = 6L;
		double num2 = p[0] * _metersPerUnit - false_easting;
		double num3 = p[1] * _metersPerUnit - false_northing;
		double num4 = (ml0 + num3 / scale_factor) / _semiMajor;
		double num5 = num4;
		long num6 = 0L;
		while (true)
		{
			double num7 = (num4 + e1 * Math.Sin(2.0 * num5) - e2 * Math.Sin(4.0 * num5) + e3 * Math.Sin(6.0 * num5)) / e0 - num5;
			num5 += num7;
			if (Math.Abs(num7) <= 1E-10)
			{
				break;
			}
			if (num6 >= num)
			{
				throw new ArgumentException("Latitude failed to converge");
			}
			num6++;
		}
		if (Math.Abs(num5) < Math.PI / 2.0)
		{
			MapProjection.sincos(num5, out var sin_val, out var cos_val);
			double num8 = Math.Tan(num5);
			double num9 = esp * Math.Pow(cos_val, 2.0);
			double num10 = Math.Pow(num9, 2.0);
			double num11 = Math.Pow(num8, 2.0);
			double num12 = Math.Pow(num11, 2.0);
			num4 = 1.0 - es * Math.Pow(sin_val, 2.0);
			double num13 = _semiMajor / Math.Sqrt(num4);
			double num14 = num13 * (1.0 - es) / num4;
			double num15 = num2 / (num13 * scale_factor);
			double num16 = Math.Pow(num15, 2.0);
			double rad = num5 - num13 * num8 * num16 / num14 * (0.5 - num16 / 24.0 * (5.0 + 3.0 * num11 + 10.0 * num9 - 4.0 * num10 - 9.0 * esp - num16 / 30.0 * (61.0 + 90.0 * num11 + 298.0 * num9 + 45.0 * num12 - 252.0 * esp - 3.0 * num10)));
			double rad2 = MapProjection.adjust_lon(central_meridian + num15 * (1.0 - num16 / 6.0 * (1.0 + 2.0 * num11 + num9 - num16 / 20.0 * (5.0 - 2.0 * num9 + 28.0 * num11 - 3.0 * num10 + 8.0 * esp + 24.0 * num12))) / cos_val);
			if (p.Length < 3)
			{
				return new double[2]
				{
					MathTransform.Radians2Degrees(rad2),
					MathTransform.Radians2Degrees(rad)
				};
			}
			return new double[3]
			{
				MathTransform.Radians2Degrees(rad2),
				MathTransform.Radians2Degrees(rad),
				p[2]
			};
		}
		if (p.Length < 3)
		{
			return new double[2]
			{
				MathTransform.Radians2Degrees(Math.PI / 2.0 * MapProjection.sign(num3)),
				MathTransform.Radians2Degrees(central_meridian)
			};
		}
		return new double[3]
		{
			MathTransform.Radians2Degrees(Math.PI / 2.0 * MapProjection.sign(num3)),
			MathTransform.Radians2Degrees(central_meridian),
			p[2]
		};
	}

	public override IMathTransform Inverse()
	{
		if (_inverse == null)
		{
			_inverse = new TransverseMercator(_Parameters, !_isInverse);
		}
		return _inverse;
	}
}
