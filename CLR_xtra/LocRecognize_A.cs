using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
{
    /* Function to return location entity type from Azure Maps Reverse Geocoding API */
    public static string LocRec(string longitude, string latitude, string azureMapsKey)
    {
        string jsonResponse = string.Empty;

        // URL template for making an API request to Azure Maps Reverse Geocoding API
        string urlTemplate = "https://atlas.microsoft.com/search/address/reverse/json?api-version=1.0&query={1},{0}&subscription-key={2}";
        string url = string.Format(urlTemplate, longitude, latitude, azureMapsKey.Trim());

        // Make request to the Azure Maps API
        HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
        webrequest.Method = "GET";

        // Retrieve the response
        using (HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse())
        using (Stream stream = webresponse.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        {
            jsonResponse = reader.ReadToEnd();
        }

        // Parse JSON response
        using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
        {
            JsonElement root = doc.RootElement;
            if (root.TryGetProperty("addresses", out JsonElement addresses) && addresses.GetArrayLength() > 0)
            {
                JsonElement firstAddress = addresses[0];
                if (firstAddress.TryGetProperty("entityType", out JsonElement entityType))
                {
                    return entityType.GetString();
                }
            }
        }

        return null; // Return null if no valid response
    }

    /* Wrapper method to expose API functionality as SQL Server User-Defined Function (UDF) */
    [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlString loc_recognize(SqlDecimal lng, SqlDecimal lat, SqlString key)
    {
        string longitude = lng.ToString();
        string latitude = lat.ToString();
        string azureMapsKey = key.ToString();

        string locationType = LocRec(longitude, latitude, azureMapsKey);
        return locationType != null ? new SqlString(locationType) : SqlString.Null;
    }
}
