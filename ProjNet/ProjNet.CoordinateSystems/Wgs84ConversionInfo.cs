using System;
using System.Globalization;

namespace ProjNet.CoordinateSystems;

public class Wgs84ConversionInfo : IEquatable<Wgs84ConversionInfo>
{
	private const double SEC_TO_RAD = 4.84813681109536E-06;

	public double Dx;

	public double Dy;

	public double Dz;

	public double Ex;

	public double Ey;

	public double Ez;

	public double Ppm;

	public string AreaOfUse;

	public string WKT => string.Format(CultureInfo.InvariantCulture.NumberFormat, "TOWGS84[{0}, {1}, {2}, {3}, {4}, {5}, {6}]", Dx, Dy, Dz, Ex, Ey, Ez, Ppm);

	public string XML => string.Format(CultureInfo.InvariantCulture.NumberFormat, "<CS_WGS84ConversionInfo Dx=\"{0}\" Dy=\"{1}\" Dz=\"{2}\" Ex=\"{3}\" Ey=\"{4}\" Ez=\"{5}\" Ppm=\"{6}\" />", Dx, Dy, Dz, Ex, Ey, Ez, Ppm);

	public bool HasZeroValuesOnly
	{
		get
		{
			if (Dx == 0.0 && Dy == 0.0 && Dz == 0.0 && Ex == 0.0 && Ey == 0.0 && Ez == 0.0)
			{
				return Ppm == 0.0;
			}
			return false;
		}
	}

	public Wgs84ConversionInfo()
		: this(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, string.Empty)
	{
	}

	public Wgs84ConversionInfo(double dx, double dy, double dz, double ex, double ey, double ez, double ppm)
		: this(dx, dy, dz, ex, ey, ez, ppm, string.Empty)
	{
	}

	public Wgs84ConversionInfo(double dx, double dy, double dz, double ex, double ey, double ez, double ppm, string areaOfUse)
	{
		Dx = dx;
		Dy = dy;
		Dz = dz;
		Ex = ex;
		Ey = ey;
		Ez = ez;
		Ppm = ppm;
		AreaOfUse = areaOfUse;
	}

	internal double[] GetAffineTransform()
	{
		double num = 1.0 + Ppm * 1E-06;
		return new double[7]
		{
			num,
			Ex * 4.84813681109536E-06 * num,
			Ey * 4.84813681109536E-06 * num,
			Ez * 4.84813681109536E-06 * num,
			Dx,
			Dy,
			Dz
		};
	}

	public override string ToString()
	{
		return WKT;
	}

	public override bool Equals(object obj)
	{
		return Equals(obj as Wgs84ConversionInfo);
	}

	public override int GetHashCode()
	{
		return Dx.GetHashCode() ^ Dy.GetHashCode() ^ Dz.GetHashCode() ^ Ex.GetHashCode() ^ Ey.GetHashCode() ^ Ez.GetHashCode() ^ Ppm.GetHashCode();
	}

	public bool Equals(Wgs84ConversionInfo obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (obj.Dx == Dx && obj.Dy == Dy && obj.Dz == Dz && obj.Ex == Ex && obj.Ey == Ey && obj.Ez == Ez)
		{
			return obj.Ppm == Ppm;
		}
		return false;
	}
}
