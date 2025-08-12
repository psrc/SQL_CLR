using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
{
    /* Generic function to return distance and travel time estimate from Bing distance matrix */
    public static XmlDocument LocRec(
        string origin_longitude,
        string origin_latitude,
        string dest_longitude,
        string dest_latitude,
        string mode,
        string bingKey
    )
    {
        // Variable to hold the response
        var xmlResponse = new XmlDocument();

        try
        {
            // URL template for making an api request
            string urltemplate = "http://dev.virtualearth.net/REST/V1/Routes/{4}?wp.0={1},{0}&wp.1={3},{2}&distanceUnit=mi&optmz=distance&output=xml&key={5}";

            // Insert the supplied parameters into the URL template
            string url = string.Format(urltemplate, origin_longitude, origin_latitude, dest_longitude, dest_latitude, mode?.Trim() ?? "", bingKey?.Trim() ?? "");

            // Make request to the Locations API REST service
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "GET";
            webrequest.ContentLength = 0;
            webrequest.Timeout = 30000; // 30 second timeout

            // Retrieve the response
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
            // Create a valid XML document that will signal an error condition
            xmlResponse.LoadXml($"<Response><StatusCode>500</StatusCode><ErrorDetails>{ex.Message}</ErrorDetails></Response>");
        }

        // Return an XMLDocument with the results or error
        return xmlResponse;
    }

    /* Wrapper method to expose api functionality as SQL Server User-Defined Function (UDF) */
    [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlString route_mi_min(
        SqlDecimal o_lng,
        SqlDecimal o_lat,
        SqlDecimal d_lng,
        SqlDecimal d_lat,
        SqlString tmode,
        SqlString key_b
        )
    {
        try
        {
            // Default is NULL
            SqlString distSpeed = SqlString.Null;

            // Check if any input is NULL
            if (o_lng.IsNull || o_lat.IsNull || d_lng.IsNull || d_lat.IsNull || tmode.IsNull || key_b.IsNull)
            {
                return SqlString.Null;
            }

            // Convert SqlServer datatypes to C# and call function above
            string origin_longitude = o_lng.ToString();
            string origin_latitude = o_lat.ToString();
            string dest_longitude = d_lng.ToString();
            string dest_latitude = d_lat.ToString();
            string mode = tmode.ToString();
            string bingKey = key_b.ToString();

            XmlDocument apiResponse = LocRec(
                origin_longitude, origin_latitude, dest_longitude, dest_latitude, mode, bingKey
            );

            // Safe XML navigation
            XmlNodeList statusNodes = apiResponse.GetElementsByTagName("StatusCode");
            if (statusNodes == null || statusNodes.Count == 0 || statusNodes[0].InnerText != "200")
            {
                // If response from server is invalid, return NULL
                return SqlString.Null;
            }

            XmlNodeList resourceSets = apiResponse.GetElementsByTagName("ResourceSets");
            if (resourceSets == null || resourceSets.Count == 0)
            {
                return SqlString.Null;
            }

            // Safely navigate the XML hierarchy using XPath instead of direct indexing
            XmlNode resourceSet = resourceSets[0].SelectSingleNode("ResourceSet");
            if (resourceSet == null)
            {
                return SqlString.Null;
            }

            XmlNode resources = resourceSet.SelectSingleNode("Resources");
            if (resources == null)
            {
                return SqlString.Null;
            }

            XmlNode route = resources.SelectSingleNode("Route");
            if (route == null)
            {
                return SqlString.Null;
            }

            XmlNode travelDistanceNode = route.SelectSingleNode("TravelDistance");
            XmlNode travelDurationNode = route.SelectSingleNode("TravelDuration");

            if (travelDistanceNode != null && travelDurationNode != null)
            {
                string travelMiles = travelDistanceNode.InnerText;
                string travelSeconds = travelDurationNode.InnerText;

                // Validate that the values can be parsed as numbers
                if (double.TryParse(travelSeconds, out double seconds) && double.TryParse(travelMiles, out _))
                {
                    double convertMinutes = seconds / 60.00;
                    string travelMinutes = convertMinutes.ToString();
                    distSpeed = new SqlString($"{travelMiles},{travelMinutes}");
                }
            }

            return distSpeed;
        }
        catch (Exception)
        {
            // Catch any unexpected exceptions and return NULL
            return SqlString.Null;
        }
    }
}
