namespace ProjNet.CoordinateSystems;

public interface ILinearUnit : IUnit, IInfo
{
	double MetersPerUnit { get; set; }
}
