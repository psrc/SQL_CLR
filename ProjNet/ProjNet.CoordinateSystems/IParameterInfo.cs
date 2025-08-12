using System.Collections.Generic;

namespace ProjNet.CoordinateSystems;

public interface IParameterInfo
{
	int NumParameters { get; }

	List<Parameter> Parameters { get; set; }

	Parameter[] DefaultParameters();

	Parameter GetParameterByName(string name);
}
