using System.Collections.Generic;

namespace ProjNet.CoordinateSystems.Transformations;

public interface IMathTransform
{
	int DimSource { get; }

	int DimTarget { get; }

	string WKT { get; }

	string XML { get; }

	bool Identity();

	double[,] Derivative(double[] point);

	List<double> GetCodomainConvexHull(List<double> points);

	DomainFlags GetDomainFlags(List<double> points);

	IMathTransform Inverse();

	double[] Transform(double[] point);

	List<double[]> TransformList(List<double[]> points);

	void Invert();
}
