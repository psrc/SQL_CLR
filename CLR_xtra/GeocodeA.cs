using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using System;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using Newtonsoft.Json.Linq;
using OSGeo.OSR;

public partial class UserDefinedFunctions
{
    public static JObject GeocodeAzure(
        string addressLine,
        string locality,
        string postalCode,
        string azureKey
    )
    {
        string urlTemplate = "https://atlas.microsoft.com/search/address/json?api-version=1.0&subscription-key={3}&query={0}, {1}, {2}&countrySet=US";
        string url = string.Format(urlTemplate, Uri.EscapeDataString(addressLine.Trim()), Uri.EscapeDataString(locality.Trim()), Uri.EscapeDataString(postalCode.Trim()), azureKey.Trim());

        HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
        webrequest.Method = "GET";

        HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
        StreamReader streamReader = new StreamReader(webresponse.GetResponseStream());
        string jsonResponse = streamReader.ReadToEnd();

        webresponse.Close();
        streamReader.Dispose();

        return JObject.Parse(jsonResponse);
    }

    [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlGeometry geocode_a(
        SqlString address_a,
        SqlString city_a,
        SqlString zip_a,
        SqlString key_a
    )
    {
        JObject geocodeResponse = GeocodeAzure((string)address_a ?? "", (string)city_a ?? "", (string)zip_a ?? "", (string)key_a ?? "");

        if (geocodeResponse["results"] == null || !geocodeResponse["results"].Any())
        {
            return SqlGeometry.Null;
        }

        JObject location = (JObject)geocodeResponse["results"][0];
        string precision = location["position"]?["calculationMethod"]?.ToString() ?? "";
        string confidence = location["score"]?.ToString() ?? "";
        string entity = location["entityType"]?.ToString() ?? "";
        string state = location["address"]?["countrySubdivision"]?.ToString() ?? "";

        if ((precision != "Parcel" && precision != "Rooftop") || state != "WA" || entity != "Address")
        {
            return SqlGeometry.Null;
        }

        double lon = (double)location["position"]["lon"];
        double lat = (double)location["position"]["lat"];

        string epsg_wgs1984_proj4 = @"+proj=latlong +datum=WGS84 +no_defs";
        OSGeo.OSR.SpatialReference src = new OSGeo.OSR.SpatialReference("");
        src.ImportFromProj4(epsg_wgs1984_proj4);

        string epsg_2285_proj4 = @"+proj=lcc +lat_1=48.73333333333333 +lat_2=47.5 +lat_0=47 +lon_0=-120.8333333333333 +x_0=500000.0001016001 +y_0=0 +ellps=GRS80 +datum=NAD83 +to_meter=0.3048006096012192 +no_defs";
        OSGeo.OSR.SpatialReference dst = new OSGeo.OSR.SpatialReference("");
        dst.ImportFromProj4(epsg_2285_proj4);

        OSGeo.OSR.CoordinateTransformation ct = new OSGeo.OSR.CoordinateTransformation(src, dst);
        double[] coords = new double[] { lon, lat };
        ct.TransformPoint(coords);

        return SqlGeometry.Point(coords[0], coords[1], 2285);
    }
}
