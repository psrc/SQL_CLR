namespace ProjNet.CoordinateSystems;

public interface IProjection : IInfo
{
	int NumParameters { get; }

	string ClassName { get; }

	ProjectionParameter GetParameter(int n);

	ProjectionParameter GetParameter(string name);
}
