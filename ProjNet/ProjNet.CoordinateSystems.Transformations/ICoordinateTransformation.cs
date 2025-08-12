namespace ProjNet.CoordinateSystems.Transformations;

public interface ICoordinateTransformation
{
	string AreaOfUse { get; }

	string Authority { get; }

	long AuthorityCode { get; }

	IMathTransform MathTransform { get; }

	string Name { get; }

	string Remarks { get; }

	ICoordinateSystem SourceCS { get; }

	ICoordinateSystem TargetCS { get; }

	TransformType TransformType { get; }
}
