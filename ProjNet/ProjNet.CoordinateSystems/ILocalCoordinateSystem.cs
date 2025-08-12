namespace ProjNet.CoordinateSystems;

public interface ILocalCoordinateSystem : ICoordinateSystem, IInfo
{
	ILocalDatum LocalDatum { get; set; }
}
