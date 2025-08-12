namespace ProjNet.CoordinateSystems;

public interface ICompoundCoordinateSystem : ICoordinateSystem, IInfo
{
	CoordinateSystem HeadCS { get; }

	CoordinateSystem TailCS { get; }
}
