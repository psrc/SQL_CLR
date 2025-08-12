namespace ProjNet.CoordinateSystems;

public interface IGeocentricCoordinateSystem : ICoordinateSystem, IInfo
{
	IHorizontalDatum HorizontalDatum { get; set; }

	ILinearUnit LinearUnit { get; set; }

	IPrimeMeridian PrimeMeridian { get; set; }
}
