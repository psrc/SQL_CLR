using System.Collections.Generic;

namespace ProjNet.CoordinateSystems.Transformations;

public interface IMathTransformFactory
{
	MathTransform CreateAffineTransform(double[,] matrix);

	MathTransform CreateConcatenatedTransform(MathTransform transform1, MathTransform transform2);

	MathTransform CreateFromWKT(string wkt);

	MathTransform CreateFromXML(string xml);

	MathTransform CreateParameterizedTransform(string classification, List<Parameter> parameters);

	MathTransform CreatePassThroughTransform(int firstAffectedOrdinate, MathTransform subTransform);

	bool IsParameterAngular(string parameterName);

	bool IsParameterLinear(string parameterName);
}
