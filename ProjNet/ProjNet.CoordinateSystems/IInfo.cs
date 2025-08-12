namespace ProjNet.CoordinateSystems;

public interface IInfo
{
	string Name { get; }

	string Authority { get; }

	long AuthorityCode { get; }

	string Alias { get; }

	string Abbreviation { get; }

	string Remarks { get; }

	string WKT { get; }

	string XML { get; }

	bool EqualParams(object obj);
}
