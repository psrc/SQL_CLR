using System;
using System.Collections.Generic;
using ProjNet.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections;

internal class LambertConformalConic2SP : MapProjection
{
	private double _falseEasting;

	private double _falseNorthing;

	private double es;

	private double e;

	private double center_lon;

	private double center_lat;

	private double ns;

	private double f0;

	private double rh;

	public LambertConformalConic2SP(List<ProjectionParameter> parameters)
		: this(parameters, isInverse: false)
	{
	}

	public LambertConformalConic2SP(List<ProjectionParameter> parameters, bool isInverse)
		: base(parameters, isInverse)
	{
		base.Name = "Lambert_Conformal_Conic_2SP";
		base.Authority = "EPSG";
		base.AuthorityCode = 9802L;
		ProjectionParameter parameter = GetParameter("latitude_of_origin");
		ProjectionParameter parameter2 = GetParameter("central_meridian");
		ProjectionParameter parameter3 = GetParameter("standard_parallel_1");
		ProjectionParameter parameter4 = GetParameter("standard_parallel_2");
		ProjectionParameter parameter5 = GetParameter("false_easting");
		ProjectionParameter parameter6 = GetParameter("false_northing");
		if (parameter == null)
		{
			throw new ArgumentException("Missing projection parameter 'latitude_of_origin'");
		}
		if (parameter2 == null)
		{
			throw new ArgumentException("Missing projection parameter 'central_meridian'");
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
		double num = MathTransform.Degrees2Radians(parameter.Value);
		double num2 = MathTransform.Degrees2Radians(parameter2.Value);
		double num3 = MathTransform.Degrees2Radians(parameter3.Value);
		double num4 = MathTransform.Degrees2Radians(parameter4.Value);
		_falseEasting = parameter5.Value * _metersPerUnit;
		_falseNorthing = parameter6.Value * _metersPerUnit;
		if (Math.Abs(num3 + num4) < 1E-10)
		{
			throw new ArgumentException("Equal latitudes for St. Parallels on opposite sides of equator.");
		}
		es = 1.0 - Math.Pow(_semiMinor / _semiMajor, 2.0);
		e = Math.Sqrt(es);
		center_lon = num2;
		center_lat = num;
		MapProjection.sincos(num3, out var sin_val, out var cos_val);
		double num5 = sin_val;
		double num6 = MapProjection.msfnz(e, sin_val, cos_val);
		double num7 = MapProjection.tsfnz(e, num3, sin_val);
		MapProjection.sincos(num4, out sin_val, out cos_val);
		double num8 = MapProjection.msfnz(e, sin_val, cos_val);
		double num9 = MapProjection.tsfnz(e, num4, sin_val);
		sin_val = Math.Sin(center_lat);
		double x = MapProjection.tsfnz(e, center_lat, sin_val);
		if (Math.Abs(num3 - num4) > 1E-10)
		{
			ns = Math.Log(num6 / num8) / Math.Log(num7 / num9);
		}
		else
		{
			ns = num5;
		}
		f0 = num6 / (ns * Math.Pow(num7, ns));
		rh = _semiMajor * f0 * Math.Pow(x, ns);
	}

	public override double[] DegreesToMeters(double[] lonlat)
	{
		double num = MathTransform.Degrees2Radians(lonlat[0]);
		double num2 = MathTransform.Degrees2Radians(lonlat[1]);
		double num3 = Math.Abs(Math.Abs(num2) - Math.PI / 2.0);
		double num4;
		if (num3 > 1E-10)
		{
			double sinphi = Math.Sin(num2);
			double x = MapProjection.tsfnz(e, num2, sinphi);
			num4 = _semiMajor * f0 * Math.Pow(x, ns);
		}
		else
		{
			num3 = num2 * ns;
			if (num3 <= 0.0)
			{
				throw new ArgumentException();
			}
			num4 = 0.0;
		}
		double num5 = ns * MapProjection.adjust_lon(num - center_lon);
		num = num4 * Math.Sin(num5) + _falseEasting;
		num2 = rh - num4 * Math.Cos(num5) + _falseNorthing;
		if (lonlat.Length == 2)
		{
			return new double[2]
			{
				num / _metersPerUnit,
				num2 / _metersPerUnit
			};
		}
		return new double[3]
		{
			num / _metersPerUnit,
			num2 / _metersPerUnit,
			lonlat[2]
		};
	}

	public override double[] MetersToDegrees(double[] p)
	{
		double num = double.NaN;
		double num2 = double.NaN;
		long flag = 0L;
		double num3 = p[0] * _metersPerUnit - _falseEasting;
		double num4 = rh - p[1] * _metersPerUnit + _falseNorthing;
		double num5;
		double num6;
		if (ns > 0.0)
		{
			num5 = Math.Sqrt(num3 * num3 + num4 * num4);
			num6 = 1.0;
		}
		else
		{
			num5 = 0.0 - Math.Sqrt(num3 * num3 + num4 * num4);
			num6 = -1.0;
		}
		double num7 = 0.0;
		if (num5 != 0.0)
		{
			num7 = Math.Atan2(num6 * num3, num6 * num4);
		}
		if (num5 != 0.0 || ns > 0.0)
		{
			num6 = 1.0 / ns;
			double ts = Math.Pow(num5 / (_semiMajor * f0), num6);
			num2 = MapProjection.phi2z(e, ts, out flag);
			if (flag != 0)
			{
				throw new ArgumentException();
			}
		}
		else
		{
			num2 = -Math.PI / 2.0;
		}
		num = MapProjection.adjust_lon(num7 / ns + center_lon);
		if (p.Length == 2)
		{
			return new double[2]
			{
				MathTransform.Radians2Degrees(num),
				MathTransform.Radians2Degrees(num2)
			};
		}
		return new double[3]
		{
			MathTransform.Radians2Degrees(num),
			MathTransform.Radians2Degrees(num2),
			p[2]
		};
	}

	public override IMathTransform Inverse()
	{
		if (_inverse == null)
		{
			_inverse = new LambertConformalConic2SP(_Parameters, !_isInverse);
		}
		return _inverse;
	}
}
