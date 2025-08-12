using System;
using System.Collections.Generic;
using System.Globalization;

namespace ProjNet.CoordinateSystems;

public abstract class CoordinateSystem : Info, ICoordinateSystem, IInfo
{
	private List<AxisInfo> _AxisInfo;

	private double[] _DefaultEnvelope;

	public int Dimension => _AxisInfo.Count;

	internal List<AxisInfo> AxisInfo
	{
		get
		{
			return _AxisInfo;
		}
		set
		{
			_AxisInfo = value;
		}
	}

	public double[] DefaultEnvelope
	{
		get
		{
			return _DefaultEnvelope;
		}
		set
		{
			_DefaultEnvelope = value;
		}
	}

	internal CoordinateSystem(string name, string authority, long authorityCode, string alias, string abbreviation, string remarks)
		: base(name, authority, authorityCode, alias, abbreviation, remarks)
	{
	}

	public abstract IUnit GetUnits(int dimension);

	public AxisInfo GetAxis(int dimension)
	{
		if (dimension >= _AxisInfo.Count || dimension < 0)
		{
			throw new ArgumentException("AxisInfo not available for dimension " + dimension.ToString(CultureInfo.InvariantCulture));
		}
		return _AxisInfo[dimension];
	}
}
