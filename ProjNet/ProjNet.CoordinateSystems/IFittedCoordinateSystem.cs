namespace ProjNet.CoordinateSystems;

public interface IFittedCoordinateSystem : ICoordinateSystem, IInfo
{
	ICoordinateSystem BaseCoordinateSystem { get; }

	string ToBase();
}
