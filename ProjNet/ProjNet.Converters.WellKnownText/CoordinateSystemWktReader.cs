using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ProjNet.CoordinateSystems;

namespace ProjNet.Converters.WellKnownText;

public class CoordinateSystemWktReader
{
	public static IInfo Parse(string wkt)
	{
		IInfo info = null;
		StringReader stringReader = new StringReader(wkt);
		WktStreamTokenizer wktStreamTokenizer = new WktStreamTokenizer(stringReader);
		wktStreamTokenizer.NextToken();
		string stringValue = wktStreamTokenizer.GetStringValue();
		switch (stringValue)
		{
		case "UNIT":
			info = ReadUnit(wktStreamTokenizer);
			break;
		case "SPHEROID":
			info = ReadEllipsoid(wktStreamTokenizer);
			break;
		case "DATUM":
			info = ReadHorizontalDatum(wktStreamTokenizer);
			break;
		case "PRIMEM":
			info = ReadPrimeMeridian(wktStreamTokenizer);
			break;
		case "VERT_CS":
		case "GEOGCS":
		case "PROJCS":
		case "COMPD_CS":
		case "GEOCCS":
		case "FITTED_CS":
		case "LOCAL_CS":
			info = ReadCoordinateSystem(wkt, wktStreamTokenizer);
			break;
		default:
			throw new ArgumentException($"'{stringValue}' is not recognized.");
		}
		stringReader.Close();
		return info;
	}

	private static IUnit ReadUnit(WktStreamTokenizer tokenizer)
	{
		tokenizer.ReadToken("[");
		string name = tokenizer.ReadDoubleQuotedWord();
		tokenizer.ReadToken(",");
		tokenizer.NextToken();
		double numericValue = tokenizer.GetNumericValue();
		string authority = string.Empty;
		long authorityCode = -1L;
		tokenizer.NextToken();
		if (tokenizer.GetStringValue() == ",")
		{
			tokenizer.ReadAuthority(ref authority, ref authorityCode);
			tokenizer.ReadToken("]");
		}
		return new Unit(numericValue, name, authority, authorityCode, string.Empty, string.Empty, string.Empty);
	}

	private static ILinearUnit ReadLinearUnit(WktStreamTokenizer tokenizer)
	{
		tokenizer.ReadToken("[");
		string name = tokenizer.ReadDoubleQuotedWord();
		tokenizer.ReadToken(",");
		tokenizer.NextToken();
		double numericValue = tokenizer.GetNumericValue();
		string authority = string.Empty;
		long authorityCode = -1L;
		tokenizer.NextToken();
		if (tokenizer.GetStringValue() == ",")
		{
			tokenizer.ReadAuthority(ref authority, ref authorityCode);
			tokenizer.ReadToken("]");
		}
		return new LinearUnit(numericValue, name, authority, authorityCode, string.Empty, string.Empty, string.Empty);
	}

	private static IAngularUnit ReadAngularUnit(WktStreamTokenizer tokenizer)
	{
		tokenizer.ReadToken("[");
		string name = tokenizer.ReadDoubleQuotedWord();
		tokenizer.ReadToken(",");
		tokenizer.NextToken();
		double numericValue = tokenizer.GetNumericValue();
		string authority = string.Empty;
		long authorityCode = -1L;
		tokenizer.NextToken();
		if (tokenizer.GetStringValue() == ",")
		{
			tokenizer.ReadAuthority(ref authority, ref authorityCode);
			tokenizer.ReadToken("]");
		}
		return new AngularUnit(numericValue, name, authority, authorityCode, string.Empty, string.Empty, string.Empty);
	}

	private static AxisInfo ReadAxis(WktStreamTokenizer tokenizer)
	{
		if (tokenizer.GetStringValue() != "AXIS")
		{
			tokenizer.ReadToken("AXIS");
		}
		tokenizer.ReadToken("[");
		string name = tokenizer.ReadDoubleQuotedWord();
		tokenizer.ReadToken(",");
		tokenizer.NextToken();
		string stringValue = tokenizer.GetStringValue();
		tokenizer.ReadToken("]");
		return stringValue.ToUpper(CultureInfo.InvariantCulture) switch
		{
			"DOWN" => new AxisInfo(name, AxisOrientationEnum.Down), 
			"EAST" => new AxisInfo(name, AxisOrientationEnum.East), 
			"NORTH" => new AxisInfo(name, AxisOrientationEnum.North), 
			"OTHER" => new AxisInfo(name, AxisOrientationEnum.Other), 
			"SOUTH" => new AxisInfo(name, AxisOrientationEnum.South), 
			"UP" => new AxisInfo(name, AxisOrientationEnum.Up), 
			"WEST" => new AxisInfo(name, AxisOrientationEnum.West), 
			_ => throw new ArgumentException("Invalid axis name '" + stringValue + "' in WKT"), 
		};
	}

	private static ICoordinateSystem ReadCoordinateSystem(string coordinateSystem, WktStreamTokenizer tokenizer)
	{
		switch (tokenizer.GetStringValue())
		{
		case "GEOGCS":
			return ReadGeographicCoordinateSystem(tokenizer);
		case "PROJCS":
			return ReadProjectedCoordinateSystem(tokenizer);
		case "COMPD_CS":
		case "VERT_CS":
		case "GEOCCS":
		case "FITTED_CS":
		case "LOCAL_CS":
			throw new NotSupportedException($"{coordinateSystem} coordinate system is not supported.");
		default:
			throw new InvalidOperationException($"{coordinateSystem} coordinate system is not recognized.");
		}
	}

	private static Wgs84ConversionInfo ReadWGS84ConversionInfo(WktStreamTokenizer tokenizer)
	{
		tokenizer.ReadToken("[");
		Wgs84ConversionInfo wgs84ConversionInfo = new Wgs84ConversionInfo();
		tokenizer.NextToken();
		wgs84ConversionInfo.Dx = tokenizer.GetNumericValue();
		tokenizer.ReadToken(",");
		tokenizer.NextToken();
		wgs84ConversionInfo.Dy = tokenizer.GetNumericValue();
		tokenizer.ReadToken(",");
		tokenizer.NextToken();
		wgs84ConversionInfo.Dz = tokenizer.GetNumericValue();
		tokenizer.NextToken();
		if (tokenizer.GetStringValue() == ",")
		{
			tokenizer.NextToken();
			wgs84ConversionInfo.Ex = tokenizer.GetNumericValue();
			tokenizer.ReadToken(",");
			tokenizer.NextToken();
			wgs84ConversionInfo.Ey = tokenizer.GetNumericValue();
			tokenizer.ReadToken(",");
			tokenizer.NextToken();
			wgs84ConversionInfo.Ez = tokenizer.GetNumericValue();
			tokenizer.NextToken();
			if (tokenizer.GetStringValue() == ",")
			{
				tokenizer.NextToken();
				wgs84ConversionInfo.Ppm = tokenizer.GetNumericValue();
			}
		}
		if (tokenizer.GetStringValue() != "]")
		{
			tokenizer.ReadToken("]");
		}
		return wgs84ConversionInfo;
	}

	private static IEllipsoid ReadEllipsoid(WktStreamTokenizer tokenizer)
	{
		tokenizer.ReadToken("[");
		string name = tokenizer.ReadDoubleQuotedWord();
		tokenizer.ReadToken(",");
		tokenizer.NextToken();
		double numericValue = tokenizer.GetNumericValue();
		tokenizer.ReadToken(",");
		tokenizer.NextToken();
		double numericValue2 = tokenizer.GetNumericValue();
		tokenizer.NextToken();
		string authority = string.Empty;
		long authorityCode = -1L;
		if (tokenizer.GetStringValue() == ",")
		{
			tokenizer.ReadAuthority(ref authority, ref authorityCode);
			tokenizer.ReadToken("]");
		}
		return new Ellipsoid(numericValue, 0.0, numericValue2, isIvfDefinitive: true, LinearUnit.Metre, name, authority, authorityCode, string.Empty, string.Empty, string.Empty);
	}

	private static IProjection ReadProjection(WktStreamTokenizer tokenizer)
	{
		tokenizer.ReadToken("PROJECTION");
		tokenizer.ReadToken("[");
		string text = tokenizer.ReadDoubleQuotedWord();
		tokenizer.ReadToken("]");
		tokenizer.ReadToken(",");
		tokenizer.ReadToken("PARAMETER");
		List<ProjectionParameter> list = new List<ProjectionParameter>();
		while (tokenizer.GetStringValue() == "PARAMETER")
		{
			tokenizer.ReadToken("[");
			string name = tokenizer.ReadDoubleQuotedWord();
			tokenizer.ReadToken(",");
			tokenizer.NextToken();
			double numericValue = tokenizer.GetNumericValue();
			tokenizer.ReadToken("]");
			tokenizer.ReadToken(",");
			list.Add(new ProjectionParameter(name, numericValue));
			tokenizer.NextToken();
		}
		string empty = string.Empty;
		long code = -1L;
		return new Projection(text, list, text, empty, code, string.Empty, string.Empty, string.Empty);
	}

	private static IProjectedCoordinateSystem ReadProjectedCoordinateSystem(WktStreamTokenizer tokenizer)
	{
		tokenizer.ReadToken("[");
		string name = tokenizer.ReadDoubleQuotedWord();
		tokenizer.ReadToken(",");
		tokenizer.ReadToken("GEOGCS");
		IGeographicCoordinateSystem geographicCoordinateSystem = ReadGeographicCoordinateSystem(tokenizer);
		tokenizer.ReadToken(",");
		IProjection projection = ReadProjection(tokenizer);
		IUnit unit = ReadLinearUnit(tokenizer);
		string authority = string.Empty;
		long authorityCode = -1L;
		tokenizer.NextToken();
		List<AxisInfo> list = new List<AxisInfo>(2);
		if (tokenizer.GetStringValue() == ",")
		{
			tokenizer.NextToken();
			while (tokenizer.GetStringValue() == "AXIS")
			{
				list.Add(ReadAxis(tokenizer));
				tokenizer.NextToken();
				if (tokenizer.GetStringValue() == ",")
				{
					tokenizer.NextToken();
				}
			}
			if (tokenizer.GetStringValue() == ",")
			{
				tokenizer.NextToken();
			}
			if (tokenizer.GetStringValue() == "AUTHORITY")
			{
				tokenizer.ReadAuthority(ref authority, ref authorityCode);
				tokenizer.ReadToken("]");
			}
		}
		if (list.Count == 0)
		{
			list.Add(new AxisInfo("X", AxisOrientationEnum.East));
			list.Add(new AxisInfo("Y", AxisOrientationEnum.North));
		}
		return new ProjectedCoordinateSystem(geographicCoordinateSystem.HorizontalDatum, geographicCoordinateSystem, unit as LinearUnit, projection, list, name, authority, authorityCode, string.Empty, string.Empty, string.Empty);
	}

	private static IGeographicCoordinateSystem ReadGeographicCoordinateSystem(WktStreamTokenizer tokenizer)
	{
		tokenizer.ReadToken("[");
		string name = tokenizer.ReadDoubleQuotedWord();
		tokenizer.ReadToken(",");
		tokenizer.ReadToken("DATUM");
		IHorizontalDatum horizontalDatum = ReadHorizontalDatum(tokenizer);
		tokenizer.ReadToken(",");
		tokenizer.ReadToken("PRIMEM");
		IPrimeMeridian primeMeridian = ReadPrimeMeridian(tokenizer);
		tokenizer.ReadToken(",");
		tokenizer.ReadToken("UNIT");
		IAngularUnit angularUnit = ReadAngularUnit(tokenizer);
		string authority = string.Empty;
		long authorityCode = -1L;
		tokenizer.NextToken();
		List<AxisInfo> list = new List<AxisInfo>(2);
		if (tokenizer.GetStringValue() == ",")
		{
			tokenizer.NextToken();
			while (tokenizer.GetStringValue() == "AXIS")
			{
				list.Add(ReadAxis(tokenizer));
				tokenizer.NextToken();
				if (tokenizer.GetStringValue() == ",")
				{
					tokenizer.NextToken();
				}
			}
			if (tokenizer.GetStringValue() == ",")
			{
				tokenizer.NextToken();
			}
			if (tokenizer.GetStringValue() == "AUTHORITY")
			{
				tokenizer.ReadAuthority(ref authority, ref authorityCode);
				tokenizer.ReadToken("]");
			}
		}
		if (list.Count == 0)
		{
			list.Add(new AxisInfo("Lon", AxisOrientationEnum.East));
			list.Add(new AxisInfo("Lat", AxisOrientationEnum.North));
		}
		return new GeographicCoordinateSystem(angularUnit, horizontalDatum, primeMeridian, list, name, authority, authorityCode, string.Empty, string.Empty, string.Empty);
	}

	private static IHorizontalDatum ReadHorizontalDatum(WktStreamTokenizer tokenizer)
	{
		Wgs84ConversionInfo toWgs = null;
		string authority = string.Empty;
		long authorityCode = -1L;
		tokenizer.ReadToken("[");
		string name = tokenizer.ReadDoubleQuotedWord();
		tokenizer.ReadToken(",");
		tokenizer.ReadToken("SPHEROID");
		IEllipsoid ellipsoid = ReadEllipsoid(tokenizer);
		tokenizer.NextToken();
		while (tokenizer.GetStringValue() == ",")
		{
			tokenizer.NextToken();
			if (tokenizer.GetStringValue() == "TOWGS84")
			{
				toWgs = ReadWGS84ConversionInfo(tokenizer);
				tokenizer.NextToken();
			}
			else if (tokenizer.GetStringValue() == "AUTHORITY")
			{
				tokenizer.ReadAuthority(ref authority, ref authorityCode);
				tokenizer.ReadToken("]");
			}
		}
		return new HorizontalDatum(ellipsoid, toWgs, DatumType.HD_Geocentric, name, authority, authorityCode, string.Empty, string.Empty, string.Empty);
	}

	private static IPrimeMeridian ReadPrimeMeridian(WktStreamTokenizer tokenizer)
	{
		tokenizer.ReadToken("[");
		string name = tokenizer.ReadDoubleQuotedWord();
		tokenizer.ReadToken(",");
		tokenizer.NextToken();
		double numericValue = tokenizer.GetNumericValue();
		tokenizer.NextToken();
		string authority = string.Empty;
		long authorityCode = -1L;
		if (tokenizer.GetStringValue() == ",")
		{
			tokenizer.ReadAuthority(ref authority, ref authorityCode);
			tokenizer.ReadToken("]");
		}
		return new PrimeMeridian(numericValue, AngularUnit.Degrees, name, authority, authorityCode, string.Empty, string.Empty, string.Empty);
	}
}
