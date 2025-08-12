using System.Globalization;

namespace ProjNet.CoordinateSystems;

public class ProjectionParameter
{
	private string _Name;

	private double _Value;

	public string Name
	{
		get
		{
			return _Name;
		}
		set
		{
			_Name = value;
		}
	}

	public double Value
	{
		get
		{
			return _Value;
		}
		set
		{
			_Value = value;
		}
	}

	public string WKT => string.Format(CultureInfo.InvariantCulture.NumberFormat, "PARAMETER[\"{0}\", {1}]", new object[2] { Name, Value });

	public string XML => string.Format(CultureInfo.InvariantCulture.NumberFormat, "<CS_ProjectionParameter Name=\"{0}\" Value=\"{1}\"/>", new object[2] { Name, Value });

	public ProjectionParameter(string name, double value)
	{
		_Name = name;
		_Value = value;
	}
}
