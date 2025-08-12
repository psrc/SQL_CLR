using System;
using System.Collections.Generic;

namespace ProjNet.CoordinateSystems.Transformations;

public abstract class MathTransform : IMathTransform
{
	protected const double R2D = 180.0 / Math.PI;

	protected const double D2R = Math.PI / 180.0;

	public virtual int DimSource
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public virtual int DimTarget
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public abstract string WKT { get; }

	public abstract string XML { get; }

	public virtual bool Identity()
	{
		throw new NotImplementedException();
	}

	public virtual double[,] Derivative(double[] point)
	{
		throw new NotImplementedException();
	}

	public virtual List<double> GetCodomainConvexHull(List<double> points)
	{
		throw new NotImplementedException();
	}

	public virtual DomainFlags GetDomainFlags(List<double> points)
	{
		throw new NotImplementedException();
	}

	public abstract IMathTransform Inverse();

	public abstract double[] Transform(double[] point);

	public abstract List<double[]> TransformList(List<double[]> points);

	public abstract void Invert();

	protected static double Degrees2Radians(double deg)
	{
		return Math.PI / 180.0 * deg;
	}

	protected static double Radians2Degrees(double rad)
	{
		return 180.0 / Math.PI * rad;
	}
}
