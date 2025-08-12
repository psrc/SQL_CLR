namespace ProjNet.CoordinateSystems;

public interface IProjectedCoordinateSystem : IHorizontalCoordinateSystem, ICoordinateSystem, IInfo
{
	IGeographicCoordinateSystem GeographicCoordinateSystem { get; set; }

	ILinearUnit LinearUnit { get; set; }

	IProjection Projection { get; set; }
}
