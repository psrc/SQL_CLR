using System;
using Microsoft.SqlServer.Types;

namespace XY_LatLng_Debug
{
    class Program
    {
        static void Main(string[] args)
        {
            // Sample coordinates
            decimal testLng = -120.5M;  // Example longitude in WGS84
            decimal testLat = 47.5M;    // Example latitude in WGS84

            // Test ToXY (WGS84 to EPSG 2285)
            var xyResult = UserDefinedFunctions.ToXY(testLng, testLat);
            Console.WriteLine($"ToXY Result: {xyResult}");

            // Sample coordinates for ToLngLat
            decimal testX = 400000.0M; // Example X in EPSG 2285
            decimal testY = 200000.0M; // Example Y in EPSG 2285

            // Test ToLngLat (EPSG 2285 to WGS84)
            var lngLatResult = UserDefinedFunctions.ToLngLat(testX, testY);
            Console.WriteLine($"ToLngLat Result: {lngLatResult}");

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
