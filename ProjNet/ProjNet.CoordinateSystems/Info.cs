using System.Text;

namespace ProjNet.CoordinateSystems;

public abstract class Info : IInfo
{
	private string _Name;

	private string _Authority;

	private long _Code;

	private string _Alias;

	private string _Abbreviation;

	private string _Remarks;

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

	public string Authority
	{
		get
		{
			return _Authority;
		}
		set
		{
			_Authority = value;
		}
	}

	public long AuthorityCode
	{
		get
		{
			return _Code;
		}
		set
		{
			_Code = value;
		}
	}

	public string Alias
	{
		get
		{
			return _Alias;
		}
		set
		{
			_Alias = value;
		}
	}

	public string Abbreviation
	{
		get
		{
			return _Abbreviation;
		}
		set
		{
			_Abbreviation = value;
		}
	}

	public string Remarks
	{
		get
		{
			return _Remarks;
		}
		set
		{
			_Remarks = value;
		}
	}

	public abstract string WKT { get; }

	public abstract string XML { get; }

	internal string InfoXml
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("<CS_Info");
			if (AuthorityCode > 0)
			{
				stringBuilder.AppendFormat(" AuthorityCode=\"{0}\"", AuthorityCode);
			}
			if (!string.IsNullOrEmpty(Abbreviation))
			{
				stringBuilder.AppendFormat(" Abbreviation=\"{0}\"", Abbreviation);
			}
			if (!string.IsNullOrEmpty(Authority))
			{
				stringBuilder.AppendFormat(" Authority=\"{0}\"", Authority);
			}
			if (!string.IsNullOrEmpty(Name))
			{
				stringBuilder.AppendFormat(" Name=\"{0}\"", Name);
			}
			stringBuilder.Append("/>");
			return stringBuilder.ToString();
		}
	}

	internal Info(string name, string authority, long code, string alias, string abbreviation, string remarks)
	{
		_Name = name;
		_Authority = authority;
		_Code = code;
		_Alias = alias;
		_Abbreviation = abbreviation;
		_Remarks = remarks;
	}

	public override string ToString()
	{
		return WKT;
	}

	public abstract bool EqualParams(object obj);
}
