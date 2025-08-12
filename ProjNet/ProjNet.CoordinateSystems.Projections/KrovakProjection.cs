using System;
using System.Collections.Generic;
using ProjNet.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections;

internal class KrovakProjection : MapProjection
{
	private const int MAXIMUM_ITERATIONS = 15;

	private const double ITERATION_TOLERANCE = 1E-11;

	private const double S45 = 0.785398163397448;

	private double _falseEasting;

	private double _falseNorthing;

	protected double _azimuth;

	protected double _pseudoStandardParallel;

	private double _sinAzim;

	private double _cosAzim;

	private double _n;

	private double _tanS2;

	private double _alfa;

	private double _hae;

	private double _k1;

	private double _ka;

	private double _ro0;

	private double _rop;

	protected double _centralMeridian;

	protected double _latitudeOfOrigin;

	protected double _scaleFactor;

	protected double _excentricitySquared;

	protected double _excentricity;

	public KrovakProjection(List<ProjectionParameter> parameters)
		: this(parameters, isInverse: false)
	{
	}

	public KrovakProjection(List<ProjectionParameter> parameters, bool isInverse)
		: base(parameters, isInverse)
	{
		base.Name = "Krovak";
		base.Authority = "EPSG";
		base.AuthorityCode = 9819L;
		ProjectionParameter parameter = GetParameter("latitude_of_center");
		ProjectionParameter parameter2 = GetParameter("longitude_of_center");
		ProjectionParameter parameter3 = GetParameter("azimuth");
		ProjectionParameter parameter4 = GetParameter("pseudo_standard_parallel_1");
		ProjectionParameter parameter5 = GetParameter("scale_factor");
		ProjectionParameter parameter6 = GetParameter("false_easting");
		ProjectionParameter parameter7 = GetParameter("false_northing");
		if (parameter == null)
		{
			throw new ArgumentException("Missing projection parameter 'latitude_of_center'");
		}
		if (parameter2 == null)
		{
			throw new ArgumentException("Missing projection parameter 'longitude_of_center'");
		}
		if (parameter3 == null)
		{
			throw new ArgumentException("Missing projection parameter 'azimuth'");
		}
		if (parameter4 == null)
		{
			throw new ArgumentException("Missing projection parameter 'pseudo_standard_parallel_1'");
		}
		if (parameter6 == null)
		{
			throw new ArgumentException("Missing projection parameter 'false_easting'");
		}
		if (parameter7 == null)
		{
			throw new ArgumentException("Missing projection parameter 'false_northing'");
		}
		_latitudeOfOrigin = MathTransform.Degrees2Radians(parameter.Value);
		_centralMeridian = MathTransform.Degrees2Radians(24.833333333333332);
		_azimuth = MathTransform.Degrees2Radians(parameter3.Value);
		_pseudoStandardParallel = MathTransform.Degrees2Radians(parameter4.Value);
		_scaleFactor = parameter5.Value;
		_falseEasting = parameter6.Value * _metersPerUnit;
		_falseNorthing = parameter7.Value * _metersPerUnit;
		_excentricitySquared = 1.0 - _semiMinor * _semiMinor / (_semiMajor * _semiMajor);
		_excentricity = Math.Sqrt(_excentricitySquared);
		_sinAzim = Math.Sin(_azimuth);
		_cosAzim = Math.Cos(_azimuth);
		_n = Math.Sin(_pseudoStandardParallel);
		_tanS2 = Math.Tan(_pseudoStandardParallel / 2.0 + 0.785398163397448);
		double num = Math.Sin(_latitudeOfOrigin);
		double num2 = Math.Cos(_latitudeOfOrigin);
		double num3 = num2 * num2;
		_alfa = Math.Sqrt(1.0 + _excentricitySquared * (num3 * num3) / (1.0 - _excentricitySquared));
		_hae = _alfa * _excentricity / 2.0;
		double num4 = Math.Asin(num / _alfa);
		double num5 = _excentricity * num;
		double num6 = Math.Pow((1.0 - num5) / (1.0 + num5), _alfa * _excentricity / 2.0);
		_k1 = Math.Pow(Math.Tan(_latitudeOfOrigin / 2.0 + 0.785398163397448), _alfa) * num6 / Math.Tan(num4 / 2.0 + 0.785398163397448);
		_ka = Math.Pow(1.0 / _k1, -1.0 / _alfa);
		double num7 = Math.Sqrt(1.0 - _excentricitySquared) / (1.0 - _excentricitySquared * (num * num));
		_ro0 = _scaleFactor * num7 / Math.Tan(_pseudoStandardParallel);
		_rop = _ro0 * Math.Pow(_tanS2, _n);
	}

	public override double[] DegreesToMeters(double[] lonlat)
	{
		double num = MathTransform.Degrees2Radians(lonlat[0]) - _centralMeridian;
		double num2 = MathTransform.Degrees2Radians(lonlat[1]);
		double num3 = _excentricity * Math.Sin(num2);
		double num4 = Math.Pow((1.0 - num3) / (1.0 + num3), _hae);
		double num5 = 2.0 * (Math.Atan(Math.Pow(Math.Tan(num2 / 2.0 + 0.785398163397448), _alfa) / _k1 * num4) - 0.785398163397448);
		double num6 = (0.0 - num) * _alfa;
		double num7 = Math.Cos(num5);
		double num8 = Math.Asin(_cosAzim * Math.Sin(num5) + _sinAzim * num7 * Math.Cos(num6));
		double num9 = Math.Asin(num7 * Math.Sin(num6) / Math.Cos(num8));
		double num10 = _n * num9;
		double num11 = _rop / Math.Pow(Math.Tan(num8 / 2.0 + 0.785398163397448), _n);
		double num12 = (0.0 - num11 * Math.Cos(num10)) * _semiMajor;
		double num13 = (0.0 - num11 * Math.Sin(num10)) * _semiMajor;
		return new double[2] { num13, num12 };
	}

	public override double[] MetersToDegrees(double[] p)
	{
		double num = p[0] / _semiMajor;
		double num2 = p[1] / _semiMajor;
		double num3 = Math.Sqrt(num * num + num2 * num2);
		double num4 = Math.Atan2(0.0 - num, 0.0 - num2);
		double num5 = num4 / _n;
		double num6 = 2.0 * (Math.Atan(Math.Pow(_ro0 / num3, 1.0 / _n) * _tanS2) - 0.785398163397448);
		double num7 = Math.Cos(num6);
		double num8 = Math.Asin(_cosAzim * Math.Sin(num6) - _sinAzim * num7 * Math.Cos(num5));
		double num9 = _ka * Math.Pow(Math.Tan(num8 / 2.0 + 0.785398163397448), 1.0 / _alfa);
		double num10 = Math.Asin(num7 * Math.Sin(num5) / Math.Cos(num8));
		double num11 = (0.0 - num10) / _alfa;
		double num12 = 0.0;
		double num13 = num8;
		int num14 = 15;
		do
		{
			num13 = num12;
			double num15 = _excentricity * Math.Sin(num13);
			num12 = 2.0 * (Math.Atan(num9 * Math.Pow((1.0 + num15) / (1.0 - num15), _excentricity / 2.0)) - 0.785398163397448);
		}
		while (!(Math.Abs(num13 - num12) <= 1E-11) && --num14 >= 0);
		return new double[2]
		{
			MathTransform.Radians2Degrees(num11 + _centralMeridian),
			MathTransform.Radians2Degrees(num12)
		};
	}

	public override IMathTransform Inverse()
	{
		if (_inverse == null)
		{
			_inverse = new KrovakProjection(_Parameters, !_isInverse);
		}
		return _inverse;
	}
}
