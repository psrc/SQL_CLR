using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using ProjNet.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections;

internal abstract class MapProjection : MathTransform, IProjection, IInfo
{
	protected const double PI = Math.PI;

	protected const double HALF_PI = Math.PI / 2.0;

	protected const double TWO_PI = Math.PI * 2.0;

	protected const double EPSLN = 1E-10;

	protected const double S2R = 4.848136811095359E-06;

	protected const double MAX_VAL = 4.0;

	protected const double prjMAXLONG = 2147483647.0;

	protected const double DBLLONG = 4.61168601E+18;

	protected bool _isInverse;

	protected double _es;

	protected double _semiMajor;

	protected double _semiMinor;

	protected double _metersPerUnit;

	protected List<ProjectionParameter> _Parameters;

	protected MathTransform _inverse;

	private string _Abbreviation;

	private string _Alias;

	private string _Authority;

	private long _Code;

	private string _Name;

	private string _Remarks;

	public int NumParameters => _Parameters.Count;

	public string ClassName => ClassName;

	public string Abbreviation
	{
		get
		{
			return _Abbreviation;
		}
		set
		{
			_Abbreviation = value;
		}
	}

	public string Alias
	{
		get
		{
			return _Alias;
		}
		set
		{
			_Alias = value;
		}
	}

	public string Authority
	{
		get
		{
			return _Authority;
		}
		set
		{
			_Authority = value;
		}
	}

	public long AuthorityCode
	{
		get
		{
			return _Code;
		}
		set
		{
			_Code = value;
		}
	}

	public string Name
	{
		get
		{
			return _Name;
		}
		set
		{
			_Name = value;
		}
	}

	public string Remarks
	{
		get
		{
			return _Remarks;
		}
		set
		{
			_Remarks = value;
		}
	}

	public override string WKT
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (_isInverse)
			{
				stringBuilder.Append("INVERSE_MT[");
			}
			stringBuilder.AppendFormat("PARAM_MT[\"{0}\"", Name);
			for (int i = 0; i < NumParameters; i++)
			{
				stringBuilder.AppendFormat(", {0}", GetParameter(i).WKT);
			}
			stringBuilder.Append("]");
			if (_isInverse)
			{
				stringBuilder.Append("]");
			}
			return stringBuilder.ToString();
		}
	}

	public override string XML
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<CT_MathTransform>");
			if (_isInverse)
			{
				stringBuilder.AppendFormat("<CT_InverseTransform Name=\"{0}\">", ClassName);
			}
			else
			{
				stringBuilder.AppendFormat("<CT_ParameterizedMathTransform Name=\"{0}\">", ClassName);
			}
			for (int i = 0; i < NumParameters; i++)
			{
				stringBuilder.AppendFormat(GetParameter(i).XML);
			}
			if (_isInverse)
			{
				stringBuilder.Append("</CT_InverseTransform>");
			}
			else
			{
				stringBuilder.Append("</CT_ParameterizedMathTransform>");
			}
			stringBuilder.Append("</CT_MathTransform>");
			return stringBuilder.ToString();
		}
	}

	internal bool IsInverse => _isInverse;

	protected MapProjection(List<ProjectionParameter> parameters, bool isInverse)
		: this(parameters)
	{
		_isInverse = isInverse;
	}

	protected MapProjection(List<ProjectionParameter> parameters)
	{
		_Parameters = parameters;
		ProjectionParameter parameter = GetParameter("semi_major");
		ProjectionParameter parameter2 = GetParameter("semi_minor");
		if (parameter == null)
		{
			throw new ArgumentException("Missing projection parameter 'semi_major'");
		}
		if (parameter2 == null)
		{
			throw new ArgumentException("Missing projection parameter 'semi_minor'");
		}
		_semiMajor = parameter.Value;
		_semiMinor = parameter2.Value;
		ProjectionParameter parameter3 = GetParameter("unit");
		_metersPerUnit = parameter3.Value;
		_es = 1.0 - _semiMinor * _semiMinor / (_semiMajor * _semiMajor);
	}

	public ProjectionParameter GetParameter(int Index)
	{
		return _Parameters[Index];
	}

	public ProjectionParameter GetParameter(string name)
	{
		return _Parameters.Find((ProjectionParameter par) => par.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
	}

	public abstract double[] MetersToDegrees(double[] p);

	public abstract double[] DegreesToMeters(double[] lonlat);

	public override void Invert()
	{
		_isInverse = !_isInverse;
	}

	public override double[] Transform(double[] cp)
	{
		if (!_isInverse)
		{
			return DegreesToMeters(cp);
		}
		return MetersToDegrees(cp);
	}

	public override List<double[]> TransformList(List<double[]> ord)
	{
		List<double[]> list = new List<double[]>(ord.Count);
		for (int i = 0; i < ord.Count; i++)
		{
			double[] point = ord[i];
			list.Add(Transform(point));
		}
		return list;
	}

	public bool EqualParams(object obj)
	{
		if (!(obj is MapProjection))
		{
			return false;
		}
		MapProjection proj = obj as MapProjection;
		if (proj.NumParameters != NumParameters)
		{
			return false;
		}
		int i;
		for (i = 0; i < _Parameters.Count; i++)
		{
			ProjectionParameter projectionParameter = _Parameters.Find((ProjectionParameter par) => par.Name.Equals(proj.GetParameter(i).Name, StringComparison.OrdinalIgnoreCase));
			if (projectionParameter == null)
			{
				return false;
			}
			if (projectionParameter.Value != proj.GetParameter(i).Value)
			{
				return false;
			}
		}
		if (IsInverse != proj.IsInverse)
		{
			return false;
		}
		return true;
	}

	protected static double CUBE(double x)
	{
		return Math.Pow(x, 3.0);
	}

	protected static double QUAD(double x)
	{
		return Math.Pow(x, 4.0);
	}

	protected static double GMAX(ref double A, ref double B)
	{
		return Math.Max(A, B);
	}

	protected static double GMIN(ref double A, ref double B)
	{
		if (!(A < B))
		{
			return B;
		}
		return A;
	}

	protected static double IMOD(double A, double B)
	{
		return A - A / B * B;
	}

	protected static double sign(double x)
	{
		if (x < 0.0)
		{
			return -1.0;
		}
		return 1.0;
	}

	protected static double adjust_lon(double x)
	{
		long num = 0L;
		while (!(Math.Abs(x) <= Math.PI))
		{
			x = (((long)Math.Abs(x / Math.PI) < 2) ? (x - sign(x) * (Math.PI * 2.0)) : (((double)(long)Math.Abs(x / (Math.PI * 2.0)) < 2147483647.0) ? (x - (double)(long)(x / (Math.PI * 2.0)) * (Math.PI * 2.0)) : (((double)(long)Math.Abs(x / 13493037698.238832) < 2147483647.0) ? (x - (double)(long)(x / 13493037698.238832) * 13493037698.238832) : ((!((double)(long)Math.Abs(x / 2.897607777935765E+19) < 2147483647.0)) ? (x - sign(x) * (Math.PI * 2.0)) : (x - (double)(long)(x / 2.897607777935765E+19) * 2.897607777935765E+19)))));
			num++;
			if ((double)num > 4.0)
			{
				break;
			}
		}
		return x;
	}

	protected static double msfnz(double eccent, double sinphi, double cosphi)
	{
		double num = eccent * sinphi;
		return cosphi / Math.Sqrt(1.0 - num * num);
	}

	protected static double qsfnz(double eccent, double sinphi)
	{
		if (eccent > 1E-07)
		{
			double num = eccent * sinphi;
			return (1.0 - eccent * eccent) * (sinphi / (1.0 - num * num) - 0.5 / eccent * Math.Log((1.0 - num) / (1.0 + num)));
		}
		return 2.0 * sinphi;
	}

	protected static void sincos(double val, out double sin_val, out double cos_val)
	{
		sin_val = Math.Sin(val);
		cos_val = Math.Cos(val);
	}

	protected static double tsfnz(double eccent, double phi, double sinphi)
	{
		double num = eccent * sinphi;
		double y = 0.5 * eccent;
		num = Math.Pow((1.0 - num) / (1.0 + num), y);
		return Math.Tan(0.5 * (Math.PI / 2.0 - phi)) / num;
	}

	protected static double phi1z(double eccent, double qs, out long flag)
	{
		flag = 0L;
		double num = asinz(0.5 * qs);
		if (eccent < 1E-10)
		{
			return num;
		}
		double num2 = eccent * eccent;
		for (long num3 = 1L; num3 <= 25; num3++)
		{
			sincos(num, out var sin_val, out var cos_val);
			double num4 = eccent * sin_val;
			double num5 = 1.0 - num4 * num4;
			double num6 = 0.5 * num5 * num5 / cos_val * (qs / (1.0 - num2) - sin_val / num5 + 0.5 / eccent * Math.Log((1.0 - num4) / (1.0 + num4)));
			num += num6;
			if (Math.Abs(num6) <= 1E-07)
			{
				return num;
			}
		}
		throw new ArgumentException("Convergence error.");
	}

	protected static double asinz(double con)
	{
		if (Math.Abs(con) > 1.0)
		{
			con = ((!(con > 1.0)) ? (-1.0) : 1.0);
		}
		return Math.Asin(con);
	}

	protected static double phi2z(double eccent, double ts, out long flag)
	{
		flag = 0L;
		double y = 0.5 * eccent;
		double num = Math.PI / 2.0 - 2.0 * Math.Atan(ts);
		for (long num2 = 0L; num2 <= 15; num2++)
		{
			double num3 = Math.Sin(num);
			double num4 = eccent * num3;
			double num5 = Math.PI / 2.0 - 2.0 * Math.Atan(ts * Math.Pow((1.0 - num4) / (1.0 + num4), y)) - num;
			num += num5;
			if (Math.Abs(num5) <= 1E-10)
			{
				return num;
			}
		}
		throw new ArgumentException("Convergence error - phi2z-conv");
	}

	protected static double e0fn(double x)
	{
		return 1.0 - 0.25 * x * (1.0 + x / 16.0 * (3.0 + 1.25 * x));
	}

	protected static double e1fn(double x)
	{
		return 0.375 * x * (1.0 + 0.25 * x * (1.0 + 15.0 / 32.0 * x));
	}

	protected static double e2fn(double x)
	{
		return 15.0 / 256.0 * x * x * (1.0 + 0.75 * x);
	}

	protected static double e3fn(double x)
	{
		return x * x * x * 0.011393229166666666;
	}

	protected static double e4fn(double x)
	{
		double num = 1.0 + x;
		double num2 = 1.0 - x;
		return Math.Sqrt(Math.Pow(num, num) * Math.Pow(num2, num2));
	}

	protected static double mlfn(double e0, double e1, double e2, double e3, double phi)
	{
		return e0 * phi - e1 * Math.Sin(2.0 * phi) + e2 * Math.Sin(4.0 * phi) - e3 * Math.Sin(6.0 * phi);
	}

	protected static long calc_utm_zone(double lon)
	{
		return (long)((lon + 180.0) / 6.0 + 1.0);
	}

	protected static double LongitudeToRadians(double x, bool edge)
	{
		bool num;
		if (!edge)
		{
			if (x > -180.0)
			{
				num = x < 180.0;
				goto IL_003e;
			}
		}
		else if (x >= -180.0)
		{
			num = x <= 180.0;
			goto IL_003e;
		}
		goto IL_0047;
		IL_003e:
		if (num)
		{
			return MathTransform.Degrees2Radians(x);
		}
		goto IL_0047;
		IL_0047:
		throw new ArgumentOutOfRangeException("x", x.ToString(CultureInfo.InvariantCulture) + " not a valid longitude in degrees.");
	}

	protected static double LatitudeToRadians(double y, bool edge)
	{
		bool num;
		if (!edge)
		{
			if (y > -90.0)
			{
				num = y < 90.0;
				goto IL_003e;
			}
		}
		else if (y >= -90.0)
		{
			num = y <= 90.0;
			goto IL_003e;
		}
		goto IL_0047;
		IL_003e:
		if (num)
		{
			return MathTransform.Degrees2Radians(y);
		}
		goto IL_0047;
		IL_0047:
		throw new ArgumentOutOfRangeException("y", y.ToString(CultureInfo.InvariantCulture) + " not a valid latitude in degrees.");
	}
}
