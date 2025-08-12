using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ProjNet.CoordinateSystems;

public class Projection : Info, IProjection, IInfo
{
	private List<ProjectionParameter> _Parameters;

	private string _ClassName;

	public int NumParameters => _Parameters.Count;

	internal List<ProjectionParameter> Parameters
	{
		get
		{
			return _Parameters;
		}
		set
		{
			_Parameters = value;
		}
	}

	public string ClassName => _ClassName;

	public override string WKT
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("PROJECTION[\"{0}\"", base.Name);
			if (!string.IsNullOrEmpty(base.Authority) && base.AuthorityCode > 0)
			{
				stringBuilder.AppendFormat(", AUTHORITY[\"{0}\", \"{1}\"]", base.Authority, base.AuthorityCode);
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}
	}

	public override string XML
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(CultureInfo.InvariantCulture.NumberFormat, "<CS_Projection Classname=\"{0}\">{1}", new object[2] { ClassName, base.InfoXml });
			foreach (ProjectionParameter parameter in Parameters)
			{
				stringBuilder.Append(parameter.XML);
			}
			stringBuilder.Append("</CS_Projection>");
			return stringBuilder.ToString();
		}
	}

	internal Projection(string className, List<ProjectionParameter> parameters, string name, string authority, long code, string alias, string remarks, string abbreviation)
		: base(name, authority, code, alias, abbreviation, remarks)
	{
		_Parameters = parameters;
		_ClassName = className;
	}

	public ProjectionParameter GetParameter(int n)
	{
		return _Parameters[n];
	}

	public ProjectionParameter GetParameter(string name)
	{
		foreach (ProjectionParameter parameter in _Parameters)
		{
			if (parameter.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				return parameter;
			}
		}
		return null;
	}

	public override bool EqualParams(object obj)
	{
		if (!(obj is Projection))
		{
			return false;
		}
		Projection projection = obj as Projection;
		if (projection.NumParameters != NumParameters)
		{
			return false;
		}
		for (int i = 0; i < _Parameters.Count; i++)
		{
			ProjectionParameter parameter = GetParameter(projection.GetParameter(i).Name);
			if (parameter == null)
			{
				return false;
			}
			if (parameter.Value != projection.GetParameter(i).Value)
			{
				return false;
			}
		}
		return true;
	}
}
