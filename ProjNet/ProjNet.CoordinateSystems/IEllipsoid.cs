namespace ProjNet.CoordinateSystems;

public interface IEllipsoid : IInfo
{
	double SemiMajorAxis { get; set; }

	double SemiMinorAxis { get; set; }

	double InverseFlattening { get; set; }

	ILinearUnit AxisUnit { get; set; }

	bool IsIvfDefinitive { get; set; }
}
