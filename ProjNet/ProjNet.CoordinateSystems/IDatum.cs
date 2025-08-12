namespace ProjNet.CoordinateSystems;

public interface IDatum : IInfo
{
	DatumType DatumType { get; set; }
}
