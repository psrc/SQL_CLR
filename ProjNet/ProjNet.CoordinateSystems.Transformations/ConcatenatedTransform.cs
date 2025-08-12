using System;
using System.Collections.Generic;

namespace ProjNet.CoordinateSystems.Transformations;

internal class ConcatenatedTransform : MathTransform
{
	protected IMathTransform _inverse;

	private List<ICoordinateTransformation> _CoordinateTransformationList;

	public List<ICoordinateTransformation> CoordinateTransformationList
	{
		get
		{
			return _CoordinateTransformationList;
		}
		set
		{
			_CoordinateTransformationList = value;
			_inverse = null;
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

	public ConcatenatedTransform()
		: this(new List<ICoordinateTransformation>())
	{
	}

	public ConcatenatedTransform(List<ICoordinateTransformation> transformlist)
	{
		_CoordinateTransformationList = transformlist;
	}

	public override double[] Transform(double[] point)
	{
		foreach (ICoordinateTransformation coordinateTransformation in _CoordinateTransformationList)
		{
			point = coordinateTransformation.MathTransform.Transform(point);
		}
		return point;
	}

	public override List<double[]> TransformList(List<double[]> points)
	{
		List<double[]> list = new List<double[]>(points.Count);
		list.AddRange(points);
		foreach (ICoordinateTransformation coordinateTransformation in _CoordinateTransformationList)
		{
			list = coordinateTransformation.MathTransform.TransformList(list);
		}
		return list;
	}

	public override IMathTransform Inverse()
	{
		if (_inverse == null)
		{
			_inverse = Clone();
			_inverse.Invert();
		}
		return _inverse;
	}

	public override void Invert()
	{
		_CoordinateTransformationList.Reverse();
		foreach (ICoordinateTransformation coordinateTransformation in _CoordinateTransformationList)
		{
			coordinateTransformation.MathTransform.Invert();
		}
	}

	public ConcatenatedTransform Clone()
	{
		List<ICoordinateTransformation> list = new List<ICoordinateTransformation>(_CoordinateTransformationList.Count);
		foreach (ICoordinateTransformation coordinateTransformation in _CoordinateTransformationList)
		{
			list.Add(coordinateTransformation);
		}
		return new ConcatenatedTransform(list);
	}
}
