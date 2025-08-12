namespace ProjNet.CoordinateSystems;

public class Parameter
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

	public Parameter(string name, double value)
	{
		_Name = name;
		_Value = value;
	}
}
