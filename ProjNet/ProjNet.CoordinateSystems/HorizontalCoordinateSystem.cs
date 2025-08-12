using System;
using System.Collections.Generic;

namespace ProjNet.CoordinateSystems;

public abstract class HorizontalCoordinateSystem : CoordinateSystem, IHorizontalCoordinateSystem, ICoordinateSystem, IInfo
{
	private IHorizontalDatum _HorizontalDatum;

	public IHorizontalDatum HorizontalDatum
	{
		get
		{
			return _HorizontalDatum;
		}
		set
		{
			_HorizontalDatum = value;
		}
	}

	internal HorizontalCoordinateSystem(IHorizontalDatum datum, List<AxisInfo> axisInfo, string name, string authority, long code, string alias, string remarks, string abbreviation)
		: base(name, authority, code, alias, abbreviation, remarks)
	{
		_HorizontalDatum = datum;
		if (axisInfo.Count != 2)
		{
			throw new ArgumentException("Axis info should contain two axes for horizontal coordinate systems");
		}
		base.AxisInfo = axisInfo;
	}
}
