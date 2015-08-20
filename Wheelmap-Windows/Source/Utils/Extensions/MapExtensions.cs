using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml.Controls.Maps;

namespace Wheelmap_Windows.Utils.Extensions {

    public class BoundingBox {
        
    }

    public static class MapExtensions {

        public static GeoboundingBox GetBoundingBox(this MapControl map) {
            try {
                Geopoint topLeft = null;

                try {
                    map.GetLocationFromOffset(new Point(0, 0), out topLeft);
                } catch {
                    var topOfMap = new Geopoint(new BasicGeoposition() {
                        Latitude = 85,
                        Longitude = 0
                    });

                    Point topPoint;
                    map.GetOffsetFromLocation(topOfMap, out topPoint);
                    map.GetLocationFromOffset(new Point(0, topPoint.Y), out topLeft);
                }

                Geopoint bottomRight = null;
                try {
                    map.GetLocationFromOffset(new Point(map.ActualWidth, map.ActualHeight), out bottomRight);
                } catch {
                    var bottomOfMap = new Geopoint(new BasicGeoposition() {
                        Latitude = -85,
                        Longitude = 0
                    });

                    Point bottomPoint;
                    map.GetOffsetFromLocation(bottomOfMap, out bottomPoint);
                    map.GetLocationFromOffset(new Point(0, bottomPoint.Y), out bottomRight);
                }

                if (topLeft != null && bottomRight != null) {
                    return new GeoboundingBox(topLeft.Position, bottomRight.Position);
                }
            }catch(Exception e) {
                // bounds invalid
            }
            return null;
        }
    }
}
