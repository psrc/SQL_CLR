namespace ProjNet.CoordinateSystems;

public interface IAngularUnit : IUnit, IInfo
{
	double RadiansPerUnit { get; set; }
}
