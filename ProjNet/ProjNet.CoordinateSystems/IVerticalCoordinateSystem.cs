namespace ProjNet.CoordinateSystems;

public interface IVerticalCoordinateSystem : ICoordinateSystem, IInfo
{
	IVerticalDatum VerticalDatum { get; set; }

	ILinearUnit VerticalUnit { get; set; }
}
