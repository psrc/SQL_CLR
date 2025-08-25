using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.SqlServer.Server;

public partial class LocRecognizeFunctions
{
    /* Generic function to return location entity type from Google Places API Nearby Search service */
    public static XmlDocument LocRec(
        string longitude,
        string latitude,
        string googleKey
    )
    {
        // Variable to hold the API response
        var xmlResponse = new XmlDocument();

        try
        {
            // URL template for Google Places API Nearby Search - searches for businesses near coordinates
            // Note: Google Places API returns JSON by default, but we'll convert it to XML for consistency
            string urltemplate = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={1},{0}&radius=50&type=establishment&key={2}";

            // Insert the supplied parameters into the URL template
            string url = string.Format(urltemplate,
                longitude != null ? longitude : "",
                latitude != null ? latitude : "",
                googleKey != null ? googleKey.Trim() : "");

            // Make request to the Google Places API REST service
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
                
                // Convert JSON response to XML format for compatibility with existing parsing logic
                xmlResponse = ConvertGooglePlacesJsonToXml(responseText);
            }
        }
        catch (Exception ex)
        {
            // Create a valid XML document that will signal an error condition
            xmlResponse.LoadXml("<Response><StatusCode>500</StatusCode><ErrorDetails>" + ex.Message + "</ErrorDetails></Response>");
        }

        // Return an XMLDocument with the results or error
        return xmlResponse;
    }

    /* Helper function to convert Google Places JSON response to XML format compatible with existing parsing logic */
    private static XmlDocument ConvertGooglePlacesJsonToXml(string jsonResponse)
    {
        var xmlDoc = new XmlDocument();
        
        try
        {
            // Simple JSON parsing without external dependencies
            // Looking for basic structure: {"status":"OK","results":[{"name":"...","types":["..."]}]}
            
            if (jsonResponse.Contains("OK") && jsonResponse.Contains("results"))
            {
                // Success case - create compatible XML structure
                xmlDoc.LoadXml("<Response><StatusCode>200</StatusCode><ResourceSets><ResourceSet><Resources><Resource></Resource></Resources></ResourceSet></ResourceSets></Response>");
                
                // Find the best business result (skip routes and generic locations)
                int resultsKeyPos = jsonResponse.IndexOf("results");
                int resultsStart = jsonResponse.IndexOf("[", resultsKeyPos) + 1;
                string bestResult = FindBestBusinessResult(jsonResponse, resultsStart);
                
                
                if (!string.IsNullOrEmpty(bestResult))
                {
                    // Extract name
                    string name = ExtractJsonValue(bestResult, "name");
                    
                    if (!string.IsNullOrEmpty(name))
                    {
                        var nameNode = xmlDoc.CreateElement("Name");
                        nameNode.InnerText = name;
                        xmlDoc.SelectSingleNode("//Resource").AppendChild(nameNode);
                    }
                    
                    // Extract primary Google type (first in the types array)
                    string types = ExtractJsonValue(bestResult, "types");
                    
                    if (!string.IsNullOrEmpty(types))
                    {
                        string primaryType = ExtractFirstGoogleType(types);
                        
                        if (!string.IsNullOrEmpty(primaryType))
                        {
                            var entityTypeNode = xmlDoc.CreateElement("EntityType");
                            entityTypeNode.InnerText = primaryType;
                            xmlDoc.SelectSingleNode("//Resource").AppendChild(entityTypeNode);
                        }
                    }
                }
            }
            else if (jsonResponse.Contains("ZERO_RESULTS"))
            {
                // No results found
                xmlDoc.LoadXml("<Response><StatusCode>200</StatusCode><ResourceSets><ResourceSet><Resources></Resources></ResourceSet></ResourceSets></Response>");
            }
            else
            {
                // Error case
                string errorMessage = ExtractJsonValue(jsonResponse, "error_message");
                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = "Unknown Google Places API error";
                }
                xmlDoc.LoadXml($"<Response><StatusCode>400</StatusCode><ErrorDetails>{errorMessage}</ErrorDetails></Response>");
            }
        }
        catch (Exception)
        {
            // Fallback error response
            xmlDoc.LoadXml("<Response><StatusCode>500</StatusCode><ErrorDetails>Failed to parse Google Places API response</ErrorDetails></Response>");
        }
        
        return xmlDoc;
    }

    /* Find the best business result, skipping routes and generic locations */
    private static string FindBestBusinessResult(string jsonResponse, int resultsStartIndex)
    {
        int currentIndex = resultsStartIndex;
        
        while (currentIndex < jsonResponse.Length)
        {
            int resultStart = jsonResponse.IndexOf("{", currentIndex);
            if (resultStart < 0) break;
            
            // Find the matching closing brace for this result object
            int braceCount = 1;
            int resultEnd = resultStart + 1;
            while (resultEnd < jsonResponse.Length && braceCount > 0)
            {
                if (jsonResponse[resultEnd] == '"')
                {
                    // Skip over quoted strings to avoid counting braces inside strings
                    resultEnd++;
                    while (resultEnd < jsonResponse.Length && jsonResponse[resultEnd] != '"')
                    {
                        if (jsonResponse[resultEnd] == '\\') resultEnd++; // Skip escaped characters
                        resultEnd++;
                    }
                }
                else if (jsonResponse[resultEnd] == '{') braceCount++;
                else if (jsonResponse[resultEnd] == '}') braceCount--;
                resultEnd++;
            }
            
            if (braceCount == 0)
            {
                string result = jsonResponse.Substring(resultStart, resultEnd - resultStart);
                
                // Check if this result has business_status and types that indicate a real business
                if (result.Contains("\"business_status\"") && 
                    (result.Contains("\"establishment\"") || result.Contains("\"point_of_interest\"")) &&
                    !result.Contains("\"route\"") &&
                    !result.Contains("\"locality\""))
                {
                    return result;
                }
                
                currentIndex = resultEnd;
            }
            else
            {
                break; // Malformed JSON
            }
        }
        
        return "";
    }
    
    /* Simple JSON value extractor without external dependencies */
    private static string ExtractJsonValue(string json, string key)
    {
        string searchPattern = "\"" + key + "\"";
        int keyIndex = json.IndexOf(searchPattern);
        if (keyIndex < 0) return "";
        
        // Find the colon after the key (might have spaces)
        int colonIndex = json.IndexOf(":", keyIndex);
        if (colonIndex < 0) return "";
        
        int valueStart = colonIndex + 1;
        
        // Skip whitespace
        while (valueStart < json.Length && char.IsWhiteSpace(json[valueStart]))
            valueStart++;
            
        if (valueStart >= json.Length) return "";
        
        // Handle string values
        if (json[valueStart] == '"')
        {
            valueStart++; // Skip opening quote
            int valueEnd = json.IndexOf('"', valueStart);
            if (valueEnd > valueStart)
            {
                return json.Substring(valueStart, valueEnd - valueStart);
            }
        }
        // Handle array values
        else if (json[valueStart] == '[')
        {
            int bracketCount = 1;
            int arrayEnd = valueStart + 1;
            while (arrayEnd < json.Length && bracketCount > 0)
            {
                if (json[arrayEnd] == '[') bracketCount++;
                else if (json[arrayEnd] == ']') bracketCount--;
                arrayEnd++;
            }
            if (bracketCount == 0)
            {
                return json.Substring(valueStart, arrayEnd - valueStart);
            }
        }
        
        return "";
    }

    /* Wrapper method to expose api functionality as SQL Server User-Defined Function (UDF) */
    [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlString loc_recognize(
        SqlDecimal lng,
        SqlDecimal lat,
        SqlString google_key
        )
    {
        try
        {
            // Default is NULL
            SqlString locationType = SqlString.Null;

            // Check if any input is NULL
            if (lng.IsNull || lat.IsNull || google_key.IsNull)
            {
                return SqlString.Null;
            }

            // Convert SqlServer datatypes to C# and call function above
            string longitude = lng.ToString();
            string latitude = lat.ToString();
            string googleApiKey = google_key.ToString();

            XmlDocument apiResponse = LocRec(
                longitude,
                latitude,
                googleApiKey
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

            // Check for business entity type from Local Search API
            XmlNode entityTypeNode = resource.SelectSingleNode("EntityType");
            if (entityTypeNode != null)
            {
                string entityType = entityTypeNode.InnerText;
                if (!string.IsNullOrEmpty(entityType))
                {
                    // Safe substring operation to ensure SQL string length limits
                    int maxLength = Math.Min(entityType.Length, 255);
                    locationType = new SqlString(entityType.Substring(0, maxLength));
                }
            }
            else
            {
                // Fallback: check for business name if no entity type
                XmlNode nameNode = resource.SelectSingleNode("Name");
                if (nameNode != null && !string.IsNullOrEmpty(nameNode.InnerText))
                {
                    locationType = new SqlString("Business");
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

    
    /* Extract the first type from Google Places types array */
    private static string ExtractFirstGoogleType(string typesArray)
    {
        if (string.IsNullOrEmpty(typesArray)) return "";
        
        // Find first quoted string in the array
        int firstQuoteIndex = typesArray.IndexOf('"');
        if (firstQuoteIndex < 0) return "";
        
        int secondQuoteIndex = typesArray.IndexOf('"', firstQuoteIndex + 1);
        if (secondQuoteIndex < 0) return "";
        
        return typesArray.Substring(firstQuoteIndex + 1, secondQuoteIndex - firstQuoteIndex - 1);
    }
    
}
