using System.Globalization;

namespace ProjNet.CoordinateSystems;

public class AxisInfo
{
	private string _Name;

	private AxisOrientationEnum _Orientation;

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

	public AxisOrientationEnum Orientation
	{
		get
		{
			return _Orientation;
		}
		set
		{
			_Orientation = value;
		}
	}

	public string WKT => $"AXIS[\"{Name}\", {Orientation.ToString().ToUpper(CultureInfo.InvariantCulture)}]";

	public string XML => string.Format(CultureInfo.InvariantCulture.NumberFormat, "<CS_AxisInfo Name=\"{0}\" Orientation=\"{1}\"/>", new object[2]
	{
		Name,
		Orientation.ToString().ToUpper(CultureInfo.InvariantCulture)
	});

	public AxisInfo(string name, AxisOrientationEnum orientation)
	{
		_Name = name;
		_Orientation = orientation;
	}
}
