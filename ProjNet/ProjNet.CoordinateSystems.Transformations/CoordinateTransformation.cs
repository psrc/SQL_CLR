namespace ProjNet.CoordinateSystems.Transformations;

public class CoordinateTransformation : ICoordinateTransformation
{
	private string _AreaOfUse;

	private string _Authority;

	private long _AuthorityCode;

	private IMathTransform _MathTransform;

	private string _Name;

	private string _Remarks;

	private ICoordinateSystem _SourceCS;

	private ICoordinateSystem _TargetCS;

	private TransformType _TransformType;

	public string AreaOfUse => _AreaOfUse;

	public string Authority => _Authority;

	public long AuthorityCode => _AuthorityCode;

	public IMathTransform MathTransform => _MathTransform;

	public string Name => _Name;

	public string Remarks => _Remarks;

	public ICoordinateSystem SourceCS => _SourceCS;

	public ICoordinateSystem TargetCS => _TargetCS;

	public TransformType TransformType => _TransformType;

	internal CoordinateTransformation(ICoordinateSystem sourceCS, ICoordinateSystem targetCS, TransformType transformType, IMathTransform mathTransform, string name, string authority, long authorityCode, string areaOfUse, string remarks)
	{
		_TargetCS = targetCS;
		_SourceCS = sourceCS;
		_TransformType = transformType;
		_MathTransform = mathTransform;
		_Name = name;
		_Authority = authority;
		_AuthorityCode = authorityCode;
		_AreaOfUse = areaOfUse;
		_Remarks = remarks;
	}
}
