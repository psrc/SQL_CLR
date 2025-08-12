namespace ProjNet.CoordinateSystems;

public interface IHorizontalCoordinateSystem : ICoordinateSystem, IInfo
{
	IHorizontalDatum HorizontalDatum { get; set; }
}
