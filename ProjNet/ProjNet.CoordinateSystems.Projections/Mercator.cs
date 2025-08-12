using System;
using System.Collections.Generic;
using ProjNet.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections;

internal class Mercator : MapProjection
{
	private double _falseEasting;

	private double _falseNorthing;

	private double lon_center;

	private double lat_origin;

	private double e;

	private double e2;

	private double k0;

	public Mercator(List<ProjectionParameter> parameters)
		: this(parameters, isInverse: false)
	{
	}

	public Mercator(List<ProjectionParameter> parameters, bool isInverse)
		: base(parameters, isInverse)
	{
		base.Authority = "EPSG";
		ProjectionParameter parameter = GetParameter("central_meridian");
		ProjectionParameter parameter2 = GetParameter("latitude_of_origin");
		ProjectionParameter parameter3 = GetParameter("scale_factor");
		ProjectionParameter parameter4 = GetParameter("false_easting");
		ProjectionParameter parameter5 = GetParameter("false_northing");
		if (parameter == null)
		{
			throw new ArgumentException("Missing projection parameter 'central_meridian'");
		}
		if (parameter2 == null)
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
		lon_center = MathTransform.Degrees2Radians(parameter.Value);
		lat_origin = MathTransform.Degrees2Radians(parameter2.Value);
		_falseEasting = parameter4.Value * _metersPerUnit;
		_falseNorthing = parameter5.Value * _metersPerUnit;
		double num = _semiMinor / _semiMajor;
		e2 = 1.0 - num * num;
		e = Math.Sqrt(e2);
		if (parameter3 == null)
		{
			k0 = Math.Cos(lat_origin) / Math.Sqrt(1.0 - e2 * Math.Sin(lat_origin) * Math.Sin(lat_origin));
			base.AuthorityCode = 9805L;
			base.Name = "Mercator_2SP";
		}
		else
		{
			k0 = parameter3.Value;
			base.Name = "Mercator_1SP";
		}
		base.Authority = "EPSG";
	}

	public override double[] DegreesToMeters(double[] lonlat)
	{
		if (double.IsNaN(lonlat[0]) || double.IsNaN(lonlat[1]))
		{
			return new double[2]
			{
				double.NaN,
				double.NaN
			};
		}
		double num = MathTransform.Degrees2Radians(lonlat[0]);
		double num2 = MathTransform.Degrees2Radians(lonlat[1]);
		if (Math.Abs(Math.Abs(num2) - Math.PI / 2.0) <= 1E-10)
		{
			throw new ArgumentException("Transformation cannot be computed at the poles.");
		}
		double num3 = e * Math.Sin(num2);
		double num4 = _falseEasting + _semiMajor * k0 * (num - lon_center);
		double num5 = _falseNorthing + _semiMajor * k0 * Math.Log(Math.Tan(Math.PI / 4.0 + num2 * 0.5) * Math.Pow((1.0 - num3) / (1.0 + num3), e * 0.5));
		if (lonlat.Length < 3)
		{
			return new double[2]
			{
				num4 / _metersPerUnit,
				num5 / _metersPerUnit
			};
		}
		return new double[3]
		{
			num4 / _metersPerUnit,
			num5 / _metersPerUnit,
			lonlat[2]
		};
	}

	public override double[] MetersToDegrees(double[] p)
	{
		double num = double.NaN;
		double num2 = double.NaN;
		double num3 = p[0] * _metersPerUnit - _falseEasting;
		double num4 = p[1] * _metersPerUnit - _falseNorthing;
		double d = Math.Exp((0.0 - num4) / (_semiMajor * k0));
		double num5 = Math.PI / 2.0 - 2.0 * Math.Atan(d);
		double num6 = Math.Pow(e, 4.0);
		double num7 = Math.Pow(e, 6.0);
		double num8 = Math.Pow(e, 8.0);
		num2 = num5 + (e2 * 0.5 + 5.0 * num6 / 24.0 + num7 / 12.0 + 13.0 * num8 / 360.0) * Math.Sin(2.0 * num5) + (7.0 * num6 / 48.0 + 29.0 * num7 / 240.0 + 811.0 * num8 / 11520.0) * Math.Sin(4.0 * num5) + (7.0 * num7 / 120.0 + 81.0 * num8 / 1120.0) * Math.Sin(6.0 * num5) + 4279.0 * num8 / 161280.0 * Math.Sin(8.0 * num5);
		num = num3 / (_semiMajor * k0) + lon_center;
		if (p.Length < 3)
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
			_inverse = new Mercator(_Parameters, !_isInverse);
		}
		return _inverse;
	}
}
