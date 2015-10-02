using System;
using Windows.Devices.Geolocation;

/**
 * http://www.codecodex.com/wiki/Calculate_distance_between_two_points_on_a_globe#C.23
 */
namespace Wheelmap.Utils {

    /// <summary>  
    /// The distance type to return the results in.  
    /// </summary>  
    public enum DistanceType { Miles, Kilometers };
    public static class DistanceTypes {
        private const double LAT_DIST_PER_DEGREE_IN_KM = 111;
        private const double LAT_DIST_PER_DEGREE_IN_MILES = 69;

        public static double LatDistPerDegree(this DistanceType type) {
            switch (type) {
                case DistanceType.Miles:
                    return LAT_DIST_PER_DEGREE_IN_MILES;
                case DistanceType.Kilometers:
                    return LAT_DIST_PER_DEGREE_IN_KM;
                default:
                    throw new Exception("unkown type");
            }
        }
    }

    public static class GeoMath {

        // Semi-axes of WGS-84 geoidal reference
        private const double WGS84_a = 6378137.0; // Major semiaxis [m]
        private const double WGS84_b = 6356752.3; // Minor semiaxis [m]

        public static GeoboundingBox GetBoundingBox(BasicGeoposition point, double halfSideInKm) {
            // Bounding box surrounding the point at given coordinates,
            // assuming local approximation of Earth surface as a sphere
            // of radius given by WGS84
            var lat = ToRadian(point.Latitude);
            var lon = ToRadian(point.Longitude);
            var halfSide = 1000 * halfSideInKm;

            // Radius of Earth at given latitude
            var radius = WGS84EarthRadius(lat);
            // Radius of the parallel at given latitude
            var pradius = radius * Math.Cos(lat);

            var latMin = lat - halfSide / radius;
            var latMax = lat + halfSide / radius;
            var lonMin = lon - halfSide / pradius;
            var lonMax = lon + halfSide / pradius;
            
            return new GeoboundingBox(
                northwestCorner: new BasicGeoposition { Latitude = ToDeg(latMax), Longitude = ToDeg(lonMin) },
                southeastCorner: new BasicGeoposition { Latitude = ToDeg(latMin), Longitude = ToDeg(lonMax) }
            );
        }
        
        // Earth radius at a given latitude, according to the WGS-84 ellipsoid [m]
        private static double WGS84EarthRadius(double lat) {
            // http://en.wikipedia.org/wiki/Earth_radius
            var An = WGS84_a * WGS84_a * Math.Cos(lat);
            var Bn = WGS84_b * WGS84_b * Math.Sin(lat);
            var Ad = WGS84_a * Math.Cos(lat);
            var Bd = WGS84_b * Math.Sin(lat);
            return Math.Sqrt((An * An + Bn * Bn) / (Ad * Ad + Bd * Bd));
        }

        /// <summary>  
        /// Convert to Radians.  
        /// </summary>  
        /// <param name="val"></param>  
        /// <returns></returns>  
        public static double ToRadian(double val) {
            return (Math.PI / 180) * val;
        }

        // radians to degrees
        private static double ToDeg(double radians) {
            return 180.0 * radians / Math.PI;
        }
    }
    
    /// <summary>  
    /// Specifies a Latitude / Longitude point.  
    /// </summary>  
    public struct Position {
        public double Latitude;
        public double Longitude;

        public static Position From(Geopoint p) {
            return From(p.Position);
        }

        public static Position From(BasicGeoposition p) {
            return new Position() {
                Latitude = p.Latitude,
                Longitude = p.Longitude
            };
        }

    }

    public class Haversine {

        /// <summary>  
        /// Returns the distance in miles or kilometers of any two  
        /// latitude / longitude points.  
        /// </summary>  
        /// <param name=”pos1″></param>  
        /// <param name=”pos2″></param>  
        /// <param name=”type”></param>  
        /// <returns></returns>  
        public static double Distance(Position pos1, Position pos2, DistanceType type) {
            double R = (type == DistanceType.Miles) ? 3960 : 6371;
            double dLat = GeoMath.ToRadian(pos2.Latitude - pos1.Latitude);
            double dLon = GeoMath.ToRadian(pos2.Longitude - pos1.Longitude);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(GeoMath.ToRadian(pos1.Latitude)) * Math.Cos(GeoMath.ToRadian(pos2.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            double d = R * c;
            return d;
        }

        public static double DistanceInKm(Position pos1, Position pos2) {
            return Distance(pos1, pos2, DistanceType.Kilometers);
        }

        public static double DistanceInMeters(Position pos1, Position pos2) {
            return DistanceInKm(pos1, pos2) * 1000;
        }
        
    }
}
