namespace ProjNet.CoordinateSystems.Transformations;

public interface ICoordinateTransformationFactory
{
	ICoordinateTransformation CreateFromCoordinateSystems(ICoordinateSystem sourceCS, ICoordinateSystem targetCS);
}
