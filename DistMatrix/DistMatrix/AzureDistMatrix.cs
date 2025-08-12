using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;

public partial class UserDefinedFunctions
{
    private static readonly HttpClient httpClient = new HttpClient();

    public static async Task<string> GetAzureMapsRouteAsync(
        string originLongitude, string originLatitude,
        string destLongitude, string destLatitude,
        string mode, string azureMapsKey)
    {
        string url = $"https://atlas.microsoft.com/route/directions/json?subscription-key={azureMapsKey}&api-version=1.0&query={originLatitude},{originLongitude}:{destLatitude},{destLongitude}&travelMode={mode}&routeType=shortest";

        HttpResponseMessage response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string jsonResponse = await response.Content.ReadAsStringAsync();
        return jsonResponse;
    }

    [SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlString route_mi_min(
        SqlDecimal o_lng, SqlDecimal o_lat,
        SqlDecimal d_lng, SqlDecimal d_lat,
        SqlString tmode, SqlString key_b)
    {
        string originLongitude = o_lng.ToString();
        string originLatitude = o_lat.ToString();
        string destLongitude = d_lng.ToString();
        string destLatitude = d_lat.ToString();
        string mode = tmode.ToString();
        string azureMapsKey = key_b.ToString();

        var task = GetAzureMapsRouteAsync(originLongitude, originLatitude, destLongitude, destLatitude, mode, azureMapsKey);
        task.Wait(); // Synchronously wait for async method
        string jsonResponse = task.Result;

        if (string.IsNullOrEmpty(jsonResponse))
        {
            return SqlString.Null;
        }

        using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
        {
            var root = doc.RootElement;
            if (root.TryGetProperty("routes", out JsonElement routes) && routes.GetArrayLength() > 0)
            {
                var route = routes[0];
                if (route.TryGetProperty("summary", out JsonElement summary))
                {
                    double travelMiles = summary.GetProperty("lengthInMeters").GetDouble() * 0.000621371; // Convert meters to miles
                    double travelMinutes = summary.GetProperty("travelTimeInSeconds").GetDouble() / 60.0; // Convert seconds to minutes
                    return new SqlString($"{travelMiles},{travelMinutes}");
                }
            }
        }

        return SqlString.Null;
    }
}
