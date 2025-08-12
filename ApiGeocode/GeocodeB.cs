using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Xml;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

public partial class Geocoders
{
    public static XmlDocument GeocodeB1(
        string addressLine,
        string locality,
        string postalCode,
        string bingKey
    )
    {
        XmlDocument xmlResponse = new XmlDocument();
        string urltemplate = "http://dev.virtualearth.net/REST/v1/Locations/?countryRegion=US&adminDistrict=WA&postalCode={2}&locality={1}&addressLine={0}&key={3}&output=xml";
        string url = string.Format(urltemplate, addressLine.Trim(), locality.Trim(), postalCode.Trim(), bingKey.Trim())
            .Replace(" Hwy ", " WA-").Replace("#", "").Replace(" ", "+");

        try
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "GET";

            using (HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse())
            using (Stream stream = webresponse.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            {
                string responseText = streamReader.ReadToEnd();
                xmlResponse.LoadXml(responseText);
            }
        }
        catch (Exception ex)
        {
            xmlResponse.LoadXml("<Error><Message>" + ex.Message + "</Message></Error>");
        }

        return xmlResponse;
    }

    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlGeometry geocode_b(SqlString address_b, SqlString city_b, SqlString zip_b, SqlString key_b)
    {
        try
        {
            XmlDocument geocodeResponse = GeocodeB1(
                address_b.IsNull ? "" : address_b.Value,
                city_b.IsNull ? "" : city_b.Value,
                zip_b.IsNull ? "" : zip_b.Value,
                key_b.IsNull ? "" : key_b.Value
            );

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(geocodeResponse.NameTable);
            nsmgr.AddNamespace("ab", "http://schemas.microsoft.com/search/local/ws/rest/v1");

            XmlNode statusNode = geocodeResponse.SelectSingleNode("//ab:StatusCode", nsmgr);
            if (statusNode == null || statusNode.InnerText != "200")
            {
                return SqlGeometry.Null;
            }

            XmlNode locationNode = geocodeResponse.SelectSingleNode("//ab:Location", nsmgr);
            if (locationNode == null)
            {
                return SqlGeometry.Null;
            }

            XmlNode longitudeNode = locationNode.SelectSingleNode("ab:GeocodePoint/ab:Longitude", nsmgr);
            XmlNode latitudeNode = locationNode.SelectSingleNode("ab:GeocodePoint/ab:Latitude", nsmgr);

            if (longitudeNode == null || latitudeNode == null)
            {
                return SqlGeometry.Null;
            }

            double longitude;
            double latitude;
            if (!double.TryParse(longitudeNode.InnerText, out longitude) || !double.TryParse(latitudeNode.InnerText, out latitude))
            {
                return SqlGeometry.Null;
            }

            return TransformCoordinates(longitude, latitude);
        }
        catch (Exception)
        {
            // Return null instead of throwing an exception
            return SqlGeometry.Null;
        }
    }

    public static XmlDocument GeocodeG1(
        string addressLine,
        string locality,
        string postalCode,
        string googleKey)
    {
        XmlDocument xmlResponse = new XmlDocument();
        string urltemplate = "https://maps.googleapis.com/maps/api/geocode/xml?address={0},+{1},+WA+{2},&key={3}&sensor=false";
        string url = string.Format(urltemplate, addressLine.Trim(), locality.Trim(), postalCode.Trim(), googleKey.Trim())
            .Replace(" Hwy ", " WA-").Replace("#", "").Replace(" ", "+");

        try
        {
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "GET";

            using (HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse())
            using (Stream stream = webresponse.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            {
                string responseText = streamReader.ReadToEnd();
                xmlResponse.LoadXml(responseText);
            }
        }
        catch (Exception ex)
        {
            // Create a valid XML document for error reporting but ensure it will result in a NULL geometry
            xmlResponse.LoadXml("<GeocodeResponse><status>ERROR</status><error_message>" + ex.Message + "</error_message></GeocodeResponse>");
        }

        return xmlResponse;
    }

    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlGeometry geocode_g(SqlString address_g, SqlString city_g, SqlString zip_g, SqlString key_g)
    {
        try
        {
            XmlDocument geocodeResponse = GeocodeG1(
                address_g.IsNull ? "" : address_g.Value,
                city_g.IsNull ? "" : city_g.Value,
                zip_g.IsNull ? "" : zip_g.Value,
                key_g.IsNull ? "" : key_g.Value
            );

            // Check if response exists and has a status node
            XmlNodeList statusNodes = geocodeResponse.GetElementsByTagName("status");
            if (statusNodes.Count == 0 || statusNodes[0].InnerText != "OK")
            {
                return SqlGeometry.Null;
            }

            // Check if we have results
            XmlNodeList resultNodes = geocodeResponse.GetElementsByTagName("result");
            if (resultNodes.Count == 0)
            {
                return SqlGeometry.Null;
            }

            XmlNode resultNode = resultNodes[0];
            XmlNode geometryNode = resultNode.SelectSingleNode("geometry/location");

            if (geometryNode == null)
            {
                return SqlGeometry.Null;
            }

            XmlNode lngNode = geometryNode.SelectSingleNode("lng");
            XmlNode latNode = geometryNode.SelectSingleNode("lat");

            if (lngNode == null || latNode == null)
            {
                return SqlGeometry.Null;
            }

            double longitude;
            double latitude;
            if (!double.TryParse(lngNode.InnerText, out longitude) || !double.TryParse(latNode.InnerText, out latitude))
            {
                return SqlGeometry.Null;
            }

            return TransformCoordinates(longitude, latitude);
        }
        catch (Exception)
        {
            // Return null instead of throwing an exception
            return SqlGeometry.Null;
        }
    }

    // Extract the coordinate transformation logic to avoid duplication
    private static SqlGeometry TransformCoordinates(double longitude, double latitude)
    {
        try
        {
            var coordinateSystemFactory = new CoordinateSystemFactory();
            var epsg2285 = coordinateSystemFactory.CreateFromWkt(@"
                 PROJCS[""NAD83 / Washington North (ftUS)"",
                    GEOGCS[""NAD83"",
                        DATUM[""North_American_Datum_1983"",
                            SPHEROID[""GRS 1980"",6378137,298.257222101,
                                AUTHORITY[""EPSG"",""7019""]],
                            AUTHORITY[""EPSG"",""6269""]],
                        PRIMEM[""Greenwich"",0,
                            AUTHORITY[""EPSG"",""8901""]],
                        UNIT[""degree"",0.0174532925199433,
                            AUTHORITY[""EPSG"",""9122""]],
                        AUTHORITY[""EPSG"",""4269""]],
                    PROJECTION[""Lambert_Conformal_Conic_2SP""],
                    PARAMETER[""latitude_of_origin"",47],
                    PARAMETER[""central_meridian"",-120.833333333333],
                    PARAMETER[""standard_parallel_1"",48.7333333333333],
                    PARAMETER[""standard_parallel_2"",47.5],
                    PARAMETER[""false_easting"",1640416.667],
                    PARAMETER[""false_northing"",0],
                    UNIT[""US survey foot"",0.304800609601219,
                        AUTHORITY[""EPSG"",""9003""]],
                    AXIS[""Easting"",EAST],
                    AXIS[""Northing"",NORTH],
                    AUTHORITY[""EPSG"",""2285""]]"
            );

            var transformFactory = new CoordinateTransformationFactory();
            var transformation = transformFactory.CreateFromCoordinateSystems(GeographicCoordinateSystem.WGS84, epsg2285);

            double[] result = transformation.MathTransform.Transform(new double[] { longitude, latitude });

            return SqlGeometry.Point(result[0], result[1], 2285);
        }
        catch (Exception)
        {
            // Return null for any transformation errors
            return SqlGeometry.Null;
        }
    }
}
