using System;
using System.Xml;

class Program
{
    static void Main()
    {
        // Hardcoded test inputs
        string address = "1011 Western Ave";
        string city = "Seattle";
        string zip = "98104";
        string googleKey = "AIzaSyCH1ybBAGO0N5PU-W3OolyRHdeErz8mAfc";
        string bingKey = "AlrP-dw5WRAoOohAABv5EhKtvgp_plo8hnfBM-FJsfvi9UdFCe0AdqT7oURMTGLC";

        // Test Google Maps Geocode Function
        Console.WriteLine("\nTesting Google Geocode Function...");
        TestGeocodeG1(address, city, zip, googleKey);

        // Test Bing Maps Geocode Function
        Console.WriteLine("\nTesting Bing Geocode Function...");
        TestGeocodeB1(address, city, zip, bingKey);
    }

    static void TestGeocodeG1(string address, string city, string zip, string key)
    {
        try
        {
            XmlDocument response = Geocoders.GeocodeG1(address, city, zip, key);
            Console.WriteLine("Google Geocode Response:");
            Console.WriteLine(response.OuterXml);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GeocodeG1: {ex.Message}");
        }
    }

    static void TestGeocodeB1(string address, string city, string zip, string key)
    {
        try
        {
            XmlDocument response = Geocoders.GeocodeB1(address, city, zip, key);
            Console.WriteLine("Bing Geocode Response:");
            Console.WriteLine(response.OuterXml);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GeocodeB1: {ex.Message}");
        }
    }
}
