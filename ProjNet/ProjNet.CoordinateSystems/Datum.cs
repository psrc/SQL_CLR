namespace ProjNet.CoordinateSystems;

public abstract class Datum : Info, IDatum, IInfo
{
	private DatumType _DatumType;

	public DatumType DatumType
	{
		get
		{
			return _DatumType;
		}
		set
		{
			_DatumType = value;
		}
	}

	internal Datum(DatumType type, string name, string authority, long code, string alias, string remarks, string abbreviation)
		: base(name, authority, code, alias, abbreviation, remarks)
	{
		_DatumType = type;
	}

	public override bool EqualParams(object obj)
	{
		if (!(obj is Ellipsoid))
		{
			return false;
		}
		return (obj as Datum).DatumType == DatumType;
	}
}
