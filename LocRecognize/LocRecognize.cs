using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
{
    /* Generic function to return location entity type from Bing Maps location recognition service */
    public static XmlDocument LocRec(
        string longitude,
        string latitude,
        string bingKey
    )
    {
        // Variable to hold the API response
        var xmlResponse = new XmlDocument();

        try
        {
            // URL template for making an API request
            string urltemplate = "http://dev.virtualearth.net/REST/v1/locationrecog/{1},{0}?radius=.01&top=1&distanceunit=mi&key={2}&output=xml";

            // Insert the supplied parameters into the URL template
            string url = string.Format(urltemplate,
                longitude ?? "",
                latitude ?? "",
                bingKey?.Trim() ?? "");

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
    public static SqlString loc_recognize(
        SqlDecimal lng,
        SqlDecimal lat,
        SqlString key_b
        )
    {
        try
        {
            // Default is NULL
            SqlString locationType = SqlString.Null;

            // Check if any input is NULL
            if (lng.IsNull || lat.IsNull || key_b.IsNull)
            {
                return SqlString.Null;
            }

            // Convert SqlServer datatypes to C# and call function above
            string longitude = lng.ToString();
            string latitude = lat.ToString();
            string bingKey = key_b.ToString();

            XmlDocument apiResponse = LocRec(
                longitude,
                latitude,
                bingKey
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

            // Use XPath for safer XML navigation
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

            XmlNode resource = resources.SelectSingleNode("Resource");
            if (resource == null)
            {
                return SqlString.Null;
            }

            XmlNode isPrivateResidenceNode = resource.SelectSingleNode("IsPrivateResidence");
            if (isPrivateResidenceNode != null)
            {
                if (isPrivateResidenceNode.InnerText == "true")
                {
                    locationType = new SqlString("Residential");
                }
                else
                {
                    // Navigate to business types
                    XmlNode businessesNode = resource.SelectSingleNode("BusinessesAtLocation/BusinessLocationEntity/BusinessInfo/Types");
                    if (businessesNode != null)
                    {
                        string busType = businessesNode.InnerText;
                        // Safe substring operation
                        if (!string.IsNullOrEmpty(busType))
                        {
                            int maxLength = Math.Min(busType.Length, 255);
                            locationType = new SqlString(busType.Substring(0, maxLength));
                        }
                    }
                }
            }

            return locationType;
        }
        catch (Exception)
        {
            // Catch any unexpected exceptions and return NULL
            return SqlString.Null;
        }
    }
}
