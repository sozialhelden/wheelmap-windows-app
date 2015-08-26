using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

/**
 * http://www.codecodex.com/wiki/Calculate_distance_between_two_points_on_a_globe#C.23
 */
namespace Wheelmap_Windows.Utils {
    
    /// <summary>  
    /// The distance type to return the results in.  
    /// </summary>  
    public enum DistanceType { Miles, Kilometers };
    
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
            double dLat = toRadian(pos2.Latitude - pos1.Latitude);
            double dLon = toRadian(pos2.Longitude - pos1.Longitude);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(toRadian(pos1.Latitude)) * Math.Cos(toRadian(pos2.Latitude)) *
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
        
        /// <summary>  
        /// Convert to Radians.  
        /// </summary>  
        /// <param name="val"></param>  
        /// <returns></returns>  
        private static double toRadian(double val) {
            return (Math.PI / 180) * val;
        }
    }
}
