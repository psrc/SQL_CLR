using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using System.Xml;
using Microsoft.SqlServer.Server;

public partial class DistMatrixFunctions
{
    /* Generic function to return distance and travel time estimate from Google Routes API */
    public static string GetDistanceMatrix(
        string origin_longitude,
        string origin_latitude,
        string dest_longitude,
        string dest_latitude,
        string mode,
        string googleKey,
        string departureTimeRfc3339 = null
    )
    {
        try
        {
            // URL for Google Routes API
            string url = "https://routes.googleapis.com/directions/v2:computeRoutes";

            // Optional departure time JSON snippet
            string departureJson = string.Empty;
            if (!string.IsNullOrEmpty(departureTimeRfc3339))
            {
                departureJson = ",\n                \"departureTime\": \"" + departureTimeRfc3339 + "\"";
            }

            // Build JSON request body for Routes API (matching working PowerShell format)
            string requestBody = string.Format(@"{
                ""origin"": {
                    ""location"": {
                        ""latLng"": {
                            ""latitude"": {1},
                            ""longitude"": {0}
                        }
                    }
                },
                ""destination"": {
                    ""location"": {
                        ""latLng"": {
                            ""latitude"": {3},
                            ""longitude"": {2}
                        }
                    }
                },
                ""travelMode"": ""{4}""{5}
            }", 
                origin_longitude != null ? origin_longitude.Trim() : "",
                origin_latitude != null ? origin_latitude.Trim() : "",
                dest_longitude != null ? dest_longitude.Trim() : "",
                dest_latitude != null ? dest_latitude.Trim() : "",
                ConvertTravelMode(mode != null ? mode.Trim() : "driving"),
                departureJson
            );

            // Make request to the Google Routes API REST service
            HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
            webrequest.Method = "POST";
            webrequest.ContentType = "application/json";
            webrequest.Headers.Add("X-Goog-Api-Key", googleKey != null ? googleKey.Trim() : "");
            webrequest.Headers.Add("X-Goog-FieldMask", "routes.duration,routes.distanceMeters");
            webrequest.Timeout = 30000; // 30 second timeout

            // Write request body
            using (Stream requestStream = webrequest.GetRequestStream())
            using (StreamWriter writer = new StreamWriter(requestStream))
            {
                writer.Write(requestBody);
            }

            // Retrieve the response
            using (HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse())
            using (Stream stream = webresponse.GetResponseStream())
            using (StreamReader streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            // Return error JSON
            return "{\"error\":\"" + ex.Message.Replace("\"", "\\\"") + "\"}";
        }
    }

    /* Convert travel mode to Routes API format */
    private static string ConvertTravelMode(string mode)
    {
        switch (mode.ToLower())
        {
            case "driving":
                return "DRIVE";
            case "walking":
                return "WALK";
            case "bicycling":
                return "BICYCLE";
            case "transit":
                return "TRANSIT";
            default:
                return "DRIVE";
        }
    }

    // Format DateTime to RFC3339 (e.g., 2025-09-09T14:37:00-07:00) with minute precision
    private static string FormatDepartureTime(DateTime dt)
    {
        // If incoming DateTime.Kind is Unspecified, assume local
        if (dt.Kind == DateTimeKind.Unspecified)
        {
            dt = DateTime.SpecifyKind(dt, DateTimeKind.Local);
        }

        // Convert to local time to preserve the intended wall-clock time for transit service
        DateTimeOffset local = dt.Kind == DateTimeKind.Utc
            ? dt.ToLocalTime()
            : dt;

        // Truncate seconds and below to minute
        local = new DateTimeOffset(local.Year, local.Month, local.Day, local.Hour, local.Minute, 0, local.Offset);

        return local.ToString("yyyy'-'MM'-'dd'T'HH':'mmK");
    }

    /* Wrapper method to expose api functionality as SQL Server User-Defined Function (UDF) */
    [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlString route_mi_min(
        SqlDecimal o_lng,
        SqlDecimal o_lat,
        SqlDecimal d_lng,
        SqlDecimal d_lat,
        SqlString tmode,
        SqlString google_key,
        SqlDateTime start_time
        )
    {
        try
        {
            // Default is NULL
            SqlString distSpeed = SqlString.Null;

            // Check if any input is NULL
            if (o_lng.IsNull || o_lat.IsNull || d_lng.IsNull || d_lat.IsNull || tmode.IsNull || google_key.IsNull)
            {
                return SqlString.Null;
            }

            // Convert SqlServer datatypes to C# and call function above
            string origin_longitude = o_lng.ToString();
            string origin_latitude = o_lat.ToString();
            string dest_longitude = d_lng.ToString();
            string dest_latitude = d_lat.ToString();
            string mode = tmode.ToString();
            string googleApiKey = google_key.ToString();

            // Convert start_time (SQL datetime2) to RFC3339 with minute precision and local offset
            string departureTimeRfc3339 = start_time.IsNull ? null : FormatDepartureTime(start_time.Value);

            string jsonResponse = GetDistanceMatrix(
                origin_longitude, origin_latitude, dest_longitude, dest_latitude, mode, googleApiKey, departureTimeRfc3339
            );

            // Parse JSON response directly
            if (string.IsNullOrEmpty(jsonResponse) || jsonResponse.Contains("error"))
            {
                return SqlString.Null;
            }

            // Check if we have routes in the response
            if (!jsonResponse.Contains("routes"))
            {
                return SqlString.Null;
            }

            // Extract distance and duration values from the Routes API JSON
            // Routes API format: { "routes": [{ "distanceMeters": 1234, "duration": "720s" }] }
            // Need to extract from the first route in the routes array
            string distanceValue = ExtractRouteValue(jsonResponse, "distanceMeters");
            string durationValue = ExtractRouteValue(jsonResponse, "duration");

            if (!string.IsNullOrEmpty(distanceValue) && !string.IsNullOrEmpty(durationValue))
            {
                // Parse distance (should be a number)
                double distanceMeters;
                if (!double.TryParse(distanceValue, out distanceMeters))
                {
                    return SqlString.Null;
                }
                
                // Parse duration (Routes API returns strings like "720s")
                double durationSeconds;
                if (durationValue.EndsWith("s"))
                {
                    string durationNumeric = durationValue.Substring(0, durationValue.Length - 1);
                    if (!double.TryParse(durationNumeric, out durationSeconds))
                    {
                        return SqlString.Null;
                    }
                }
                else
                {
                    // Fallback: try parsing as number directly
                    if (!double.TryParse(durationValue, out durationSeconds))
                    {
                        return SqlString.Null;
                    }
                }
                
                // Convert meters to miles (1 meter = 0.000621371 miles)
                double miles = distanceMeters * 0.000621371;
                double minutes = durationSeconds / 60.0;
                
                // Format to match original output: "miles,minutes"
                string travelMiles = miles.ToString("F2");
                string travelMinutes = minutes.ToString("F2");
                distSpeed = new SqlString(travelMiles + "," + travelMinutes);
            }

            return distSpeed;
        }
        catch (Exception)
        {
            // Catch any unexpected exceptions and return NULL
            return SqlString.Null;
        }
    }

    /* Extract values from Routes API response (robust approach) */
    private static string ExtractRouteValue(string json, string key)
    {
        try
        {
            // Look for the quoted key first
            string keyPattern = "\"" + key + "\"";
            int keyIndex = json.IndexOf(keyPattern);
            if (keyIndex < 0) return "";
            
            // Find the colon after the key (skip any whitespace)
            int colonIndex = keyIndex + keyPattern.Length;
            while (colonIndex < json.Length && char.IsWhiteSpace(json[colonIndex]))
                colonIndex++;
                
            if (colonIndex >= json.Length || json[colonIndex] != ':')
                return "";
                
            // Skip past colon and any whitespace
            int valueStart = colonIndex + 1;
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
            // Handle numeric values
            else
            {
                int valueEnd = valueStart;
                while (valueEnd < json.Length && 
                       char.IsDigit(json[valueEnd]))
                {
                    valueEnd++;
                }
                if (valueEnd > valueStart)
                {
                    return json.Substring(valueStart, valueEnd - valueStart);
                }
            }
        }
        catch (Exception)
        {
            // Ignore parsing errors
        }
        
        return "";
    }

    /* Simple JSON value extractor for nested objects */
    private static string ExtractJsonValue(string json, string parentKey, string childKey = null)
    {
        try
        {
            // Find the parent key
            string parentPattern = "\"" + parentKey + "\"";
            int parentIndex = json.IndexOf(parentPattern);
            if (parentIndex < 0) return "";
            
            // Find the colon after the key (might have spaces)
            int colonIndex = json.IndexOf(":", parentIndex);
            if (colonIndex < 0) return "";
            
            int valueStart = colonIndex + 1;
            
            // Skip whitespace
            while (valueStart < json.Length && char.IsWhiteSpace(json[valueStart]))
                valueStart++;
                
            if (valueStart >= json.Length) return "";
            
            // If no child key, extract direct value
            if (string.IsNullOrEmpty(childKey))
            {
                if (json[valueStart] == '"')
                {
                    // String value
                    valueStart++; // Skip opening quote
                    int valueEnd = json.IndexOf('"', valueStart);
                    if (valueEnd > valueStart)
                    {
                        return json.Substring(valueStart, valueEnd - valueStart);
                    }
                }
                else
                {
                    // Numeric or boolean value - find end by comma, brace, or bracket
                    int valueEnd = valueStart;
                    while (valueEnd < json.Length && 
                           json[valueEnd] != ',' && 
                           json[valueEnd] != '}' && 
                           json[valueEnd] != ']' &&
                           !char.IsWhiteSpace(json[valueEnd]))
                    {
                        valueEnd++;
                    }
                    if (valueEnd > valueStart)
                    {
                        return json.Substring(valueStart, valueEnd - valueStart);
                    }
                }
            }
            else
            {
                // Look for child key within parent object
                if (json[valueStart] == '{')
                {
                    int objectEnd = FindMatchingBrace(json, valueStart);
                    if (objectEnd > valueStart)
                    {
                        string objectContent = json.Substring(valueStart, objectEnd - valueStart + 1);
                        return ExtractJsonValue(objectContent, childKey);
                    }
                }
            }
        }
        catch (Exception)
        {
            // Ignore parsing errors
        }
        
        return "";
    }

    /* Find matching closing brace for an opening brace */
    private static int FindMatchingBrace(string json, int openBraceIndex)
    {
        int braceCount = 1;
        int index = openBraceIndex + 1;
        
        while (index < json.Length && braceCount > 0)
        {
            if (json[index] == '{') braceCount++;
            else if (json[index] == '}') braceCount--;
            index++;
        }
        
        return braceCount == 0 ? index - 1 : -1;
    }
}
