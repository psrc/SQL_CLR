using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using System;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using OSGeo.OSR;


public partial class UserDefinedFunctions
{
    /* Generic function to return XML geocoded location from Google Maps geocoding service */
    public static XmlDocument GeocodeG1(
        string addressLine,
        string locality,
        string postalCode,
        string googleKey
    )
    {
        // Variable to hold the geocode response
        XmlDocument xmlResponse = new XmlDocument();

        // URI template for making a geocode request
        string urltemplate = "https://maps.googleapis.com/maps/api/geocode/xml?address={0},+{1},+WA+{2},&key={3}&sensor=false";

        // Insert the supplied parameters into the URL template
        string url = string.Format(urltemplate, addressLine.Trim(), locality.Trim(), postalCode.Trim(), googleKey.Trim());
        string h = url.Replace(" Hwy ", " WA-");
        string p = h.Replace("#", "");
        string v = p.Replace(" ", "+");
        url = v;

        // Make request to the Locations API REST service
        HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(url);
        webrequest.Method = "GET";
        webrequest.ContentLength = 0;

        // Retrieve the response
        HttpWebResponse webresponse = (HttpWebResponse)webrequest.GetResponse();
        Stream stream = webresponse.GetResponseStream();
        StreamReader streamReader = new StreamReader(stream);
        xmlResponse.LoadXml(streamReader.ReadToEnd());

        // Clean up
        webresponse.Close();
        stream.Dispose();
        streamReader.Dispose();

        // Return an XMLDocument with the geocoded results 
        return xmlResponse;
    }

    /* Wrapper method to expose geocoding functionality as SQL Server User-Defined Function (UDF) */
    [Microsoft.SqlServer.Server.SqlFunction(DataAccess = DataAccessKind.Read)]
    public static SqlGeometry geocode_g(
        SqlString address_g,
        SqlString city_g,
        SqlString zip_g,
        SqlString key_g
        )
    {
        // Document to hold the XML geocoded location
        XmlDocument geocodeResponse = new XmlDocument();

        // Attempt to geocode the requested address

        string ag = (string)address_g ?? "";
        string cg = (string)city_g ?? "";
        string zg = (string)zip_g ?? "";
        string kg = (string)key_g ?? "";

        geocodeResponse = GeocodeG1(
            addressLine: ag,
            locality: cg,
            postalCode: zg,
            googleKey: kg
        );

        // If response from server is invalid, return NULL

        var ostatus = geocodeResponse.GetElementsByTagName("status")[0].InnerText;

        if (ostatus != "OK")

        {
            return SqlGeometry.Null;
        }

        var otype = geocodeResponse.GetElementsByTagName("result")[0]["type"].InnerText;

        if (otype != "premise")

        {
            return SqlGeometry.Null;
        }

        else
        {
            // Retrieve the list of geocoded locations
            XmlNodeList result = geocodeResponse.GetElementsByTagName("result");
            XElement e = XElement.Load(new XmlNodeReader(geocodeResponse));

            // Instantiate regex object
            Regex r = new Regex(@"^\d+", RegexOptions.IgnoreCase);

            // Create accuracy variables for conditions
            var xg = result[0]["geometry"]["location_type"].InnerText ?? "";
            var cog = e.Descendants("type").Where(x => x.Value == "locality").Select(x => x.Parent.Element("long_name").Value).DefaultIfEmpty("").First() ?? "";
            var zog = e.Descendants("type").Where(x => x.Value == "postal_code").Select(x => x.Parent.Element("short_name").Value).DefaultIfEmpty("").First() ?? "";
            var stog = e.Descendants("type").Where(x => x.Value == "administrative_area_level_1").Select(x => x.Parent.Element("short_name").Value).DefaultIfEmpty("").First() ?? "";
            var sog = e.Descendants("type").Where(x => x.Value == "street_number").Select(x => x.Parent.Element("short_name").Value).DefaultIfEmpty("").First() ?? "";
            string sig = Convert.ToString(r.Match(ag)).Trim() ?? "";

            string PrecisionCode = Convert.ToString(xg) ?? "";
            string OutCity = Convert.ToString(cog) ?? "";
            string OutZip = Convert.ToString(zog) ?? "";
            string OutState = Convert.ToString(stog) ?? "";
            string OutStreetnum = Convert.ToString(sog) ?? "";
            string InStreetnum = sig ?? "";

            // if QC criteria are NOT met return NULL
            if ((PrecisionCode != "ROOFTOP") ||
                (OutState != "WA") ||
                ((OutCity != cg) && (OutZip != zg)) ||
                (OutStreetnum != InStreetnum)
                )
            {
                return SqlGeometry.Null;
            }
            else
            {
                // Create a geometry Point instance of the first matching location
                double[] pg = new double[2];
                pg[0] = double.Parse(result[0]["geometry"]["location"]["lng"].InnerText);
                pg[1] = double.Parse(result[0]["geometry"]["location"]["lat"].InnerText);

                //  Transforms from EPSG 4236 i.e. WGS84
                string epsg_wgs1984_proj4 = @"+proj=latlong +datum=WGS84 +no_defs";
                OSGeo.OSR.SpatialReference src = new OSGeo.OSR.SpatialReference("");
                src.ImportFromProj4(epsg_wgs1984_proj4);

                // . . . to EPSG 2285 i.e. NAD83 SP WA N
                string epsg_2285_proj4 = @"+proj=lcc +lat_1=48.73333333333333 +lat_2=47.5 +lat_0=47 +lon_0=-120.8333333333333 +x_0=500000.0001016001 +y_0=0 +ellps=GRS80 +datum=NAD83 +to_meter=0.3048006096012192 +no_defs";
                OSGeo.OSR.SpatialReference dst = new OSGeo.OSR.SpatialReference("");
                dst.ImportFromProj4(epsg_2285_proj4);

                // Init the transformer object.
                OSGeo.OSR.CoordinateTransformation ct = new OSGeo.OSR.CoordinateTransformation(src, dst);

                ct.TransformPoint(pg);

                // Return the Point to SQL Server
                return SqlGeometry.Point(pg[0], pg[1], 2285);
            }
        }
    }
};