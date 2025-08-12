using System;
using System.Collections.Generic;

namespace ProjNet.CoordinateSystems;

public class GeographicTransform : Info, IGeographicTransform, IInfo
{
	private IGeographicCoordinateSystem _SourceGCS;

	private IGeographicCoordinateSystem _TargetGCS;

	public IGeographicCoordinateSystem SourceGCS
	{
		get
		{
			return _SourceGCS;
		}
		set
		{
			_SourceGCS = value;
		}
	}

	public IGeographicCoordinateSystem TargetGCS
	{
		get
		{
			return _TargetGCS;
		}
		set
		{
			_TargetGCS = value;
		}
	}

	public IParameterInfo ParameterInfo
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override string WKT
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	public override string XML
	{
		get
		{
			throw new NotImplementedException();
		}
	}

	internal GeographicTransform(string name, string authority, long code, string alias, string remarks, string abbreviation, IGeographicCoordinateSystem sourceGCS, IGeographicCoordinateSystem targetGCS)
		: base(name, authority, code, alias, abbreviation, remarks)
	{
		_SourceGCS = sourceGCS;
		_TargetGCS = targetGCS;
	}

	public List<double[]> Forward(List<double[]> points)
	{
		throw new NotImplementedException();
	}

	public List<double[]> Inverse(List<double[]> points)
	{
		throw new NotImplementedException();
	}

	public override bool EqualParams(object obj)
	{
		if (!(obj is GeographicTransform))
		{
			return false;
		}
		GeographicTransform geographicTransform = obj as GeographicTransform;
		if (geographicTransform.SourceGCS.EqualParams(SourceGCS))
		{
			return geographicTransform.TargetGCS.EqualParams(TargetGCS);
		}
		return false;
	}
}
