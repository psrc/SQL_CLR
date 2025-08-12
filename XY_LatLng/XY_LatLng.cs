using System;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using Microsoft.SqlServer.Types;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

public partial class UserDefinedFunctions
{
    // WKT for NAD83 / Washington North (ftUS) - EPSG:2285
    private const string NAD83_WA_NORTH_WKT = @"
        PROJCS[""NAD83 / Washington North (ftUS)"",
            GEOGCS[""NAD83"",
                DATUM[""North_American_Datum_1983"",
                    SPHEROID[""GRS 1980"",6378137,298.257222101,
                        AUTHORITY[""EPSG"",""7019""]],
                    AUTHORITY[""EPSG"",""6269""]],
                PRIMEM[""Greenwich"",0,
                    AUTHORITY[""EPSG"",""8901""]],
                UNIT[""degree"",0.0174532925199433,
                    AUTHORITY[""EPSG"",""9122""]],
                AUTHORITY[""EPSG"",""4269""]],
            PROJECTION[""Lambert_Conformal_Conic_2SP""],
            PARAMETER[""latitude_of_origin"",47],
            PARAMETER[""central_meridian"",-120.833333333333],
            PARAMETER[""standard_parallel_1"",48.7333333333333],
            PARAMETER[""standard_parallel_2"",47.5],
            PARAMETER[""false_easting"",1640416.667],
            PARAMETER[""false_northing"",0],
            UNIT[""US survey foot"",0.304800609601219,
                AUTHORITY[""EPSG"",""9003""]],
            AXIS[""Easting"",EAST],
            AXIS[""Northing"",NORTH],
            AUTHORITY[""EPSG"",""2285""]]";

    /// <summary>
    /// Converts WGS84 longitude/latitude to Washington State Plane North coordinate system (EPSG:2285)
    /// </summary>
    /// <param name="lng">Longitude in decimal degrees (WGS84)</param>
    /// <param name="lat">Latitude in decimal degrees (WGS84)</param>
    /// <returns>SQL Geometry point in WA State Plane North coordinates or NULL if transformation fails</returns>
    [SqlFunction]
    public static SqlGeometry ToXY(SqlDecimal lng, SqlDecimal lat)
    {
        try
        {
            // Check for NULL inputs
            if (lng.IsNull || lat.IsNull)
            {
                return SqlGeometry.Null;
            }

            // Check if coordinates are within reasonable bounds (rough check for WGS84)
            double dLng = (double)lng.Value;
            double dLat = (double)lat.Value;

            if (dLng < -180 || dLng > 180 || dLat < -90 || dLat > 90)
            {
                return SqlGeometry.Null; // Invalid coordinate range
            }

            var wgs84 = GeographicCoordinateSystem.WGS84;

            var coordinateSystemFactory = new CoordinateSystemFactory();
            var epsg2285 = coordinateSystemFactory.CreateFromWkt(NAD83_WA_NORTH_WKT);

            var transformFactory = new CoordinateTransformationFactory();
            var transformation = transformFactory.CreateFromCoordinateSystems(wgs84, epsg2285);

            double[] result = transformation.MathTransform.Transform(new double[] { dLng, dLat });

            return SqlGeometry.Point(result[0], result[1], 2285);
        }
        catch (Exception)
        {
            // Return null for any errors in transformation
            return SqlGeometry.Null;
        }
    }

    /// <summary>
    /// Converts Washington State Plane North coordinates (EPSG:2285) to WGS84 longitude/latitude
    /// </summary>
    /// <param name="x">X coordinate (Easting) in State Plane</param>
    /// <param name="y">Y coordinate (Northing) in State Plane</param>
    /// <returns>SQL Geometry point in WGS84 coordinates or NULL if transformation fails</returns>
    [SqlFunction]
    public static SqlGeometry ToLngLat(SqlDecimal x, SqlDecimal y)
    {
        try
        {
            // Check for NULL inputs
            if (x.IsNull || y.IsNull)
            {
                return SqlGeometry.Null;
            }

            var coordinateSystemFactory = new CoordinateSystemFactory();
            var epsg2285 = coordinateSystemFactory.CreateFromWkt(NAD83_WA_NORTH_WKT);

            var wgs84 = GeographicCoordinateSystem.WGS84;

            var transformFactory = new CoordinateTransformationFactory();
            var transformation = transformFactory.CreateFromCoordinateSystems(epsg2285, wgs84);

            double[] result = transformation.MathTransform.Transform(new double[] { (double)x.Value, (double)y.Value });

            // Check if coordinates are within reasonable bounds for WGS84
            if (result[0] < -180 || result[0] > 180 || result[1] < -90 || result[1] > 90)
            {
                return SqlGeometry.Null; // Transformation produced invalid coordinates
            }

            return SqlGeometry.Point(result[0], result[1], 4326);
        }
        catch (Exception)
        {
            // Return null for any errors in transformation
            return SqlGeometry.Null;
        }
    }
}
