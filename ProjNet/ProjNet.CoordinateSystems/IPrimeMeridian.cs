namespace ProjNet.CoordinateSystems;

public interface IPrimeMeridian : IInfo
{
	double Longitude { get; set; }

	IAngularUnit AngularUnit { get; set; }
}
