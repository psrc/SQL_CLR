namespace ProjNet.CoordinateSystems;

public interface ICoordinateSystem : IInfo
{
	int Dimension { get; }

	double[] DefaultEnvelope { get; }

	AxisInfo GetAxis(int dimension);

	IUnit GetUnits(int dimension);
}
