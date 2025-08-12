using System.Collections.Generic;

namespace ProjNet.CoordinateSystems;

public interface IGeographicTransform : IInfo
{
	IGeographicCoordinateSystem SourceGCS { get; set; }

	IGeographicCoordinateSystem TargetGCS { get; set; }

	IParameterInfo ParameterInfo { get; }

	List<double[]> Forward(List<double[]> points);

	List<double[]> Inverse(List<double[]> points);
}
