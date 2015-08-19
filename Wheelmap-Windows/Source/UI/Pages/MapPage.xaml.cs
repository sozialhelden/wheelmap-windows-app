using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap_Windows.Api.Calls;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.Utils.Extensions;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap_Windows.Source.UI.Pages {
    
    public sealed partial class MapPage : Page {

        private const byte ZOOMLEVEL_MIN = 16;
        private const byte ZOOMLEVEL_MAX = 15;

        private const int MAP_ZOOM_DEFAULT = 18; // Zoon 1 is world view

        int oldZoomLevel = 1;
        Geopoint lastRequestedPosition;

        public MapPage() {
            this.InitializeComponent();

            setOsmTileSource();
            mapControl.CenterChanged += MapControl_CenterChanged;
            mapControl.ZoomLevelChanged += MapControl_ZoomLevelChanged;
            
        }

        private void MapControl_ZoomLevelChanged(MapControl sender, object args) {
            
            var zoomLevel = (int) mapControl.ZoomLevel;
            bool isZoomedEnough = true;

            // show or hide hint to zoom in
            if (zoomLevel <= ZOOMLEVEL_MAX) {
                outOfZoomView.Visibility = Visibility.Visible;
            } else {
                outOfZoomView.Visibility = Visibility.Collapsed;
            }

            if (zoomLevel < ZOOMLEVEL_MIN) {
                oldZoomLevel = zoomLevel;
                return;
            }

            if (zoomLevel < oldZoomLevel) {
                isZoomedEnough = false;
            }

            if (isZoomedEnough && zoomLevel >= oldZoomLevel) {
                oldZoomLevel = zoomLevel;
                return;
            }

            RequestUpdate();
            oldZoomLevel = zoomLevel;
        }
        
        private void MapControl_CenterChanged(MapControl sender, object args) {
            var bbox = mapControl.GetBoundingBox();
            var center = mapControl.Center;

            double minimalLatitudeSpan = Math.Abs(bbox.NorthwestCorner.Latitude - bbox.SoutheastCorner.Latitude) / 3;
            double minimalLongitudeSpan = Math.Abs(bbox.NorthwestCorner.Longitude - bbox.SoutheastCorner.Longitude) / 3;
            

            if (lastRequestedPosition != null
                    && (Math.Abs(lastRequestedPosition.Position.Latitude
                    - center.Position.Latitude) < minimalLatitudeSpan)
                    && (Math.Abs(lastRequestedPosition.Position.Longitude
                    - center.Position.Longitude) < minimalLongitudeSpan)) {
                return;
            }

            if (mapControl.ZoomLevel < ZOOMLEVEL_MIN) {
                return;
            }

            lastRequestedPosition = center;
            RequestUpdate();
        }

        private void RequestUpdate() {

            Debug.WriteLine("RequestUpdate!!!");

            /*mapControl.MapElements.Clear();
            // todo load async
            Node[] items = NodeApiClient.GetNodes(mapControl.GetBoundingBox());

            foreach (Node n in items) {
                Debug.WriteLine("Items: " + n.name);
                AddNewMapIcons(n);
            }*/
        }
        
        private void AddNewMapIcons(Node node) {
            MapIcon MapIcon1 = new MapIcon();
            MapIcon1.Location = new Geopoint(new BasicGeoposition() {
                Latitude = node.lat,
                Longitude = node.lon
            });
            MapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            MapIcon1.Title = node.name == null ? "" : node.name;

            mapControl.MapElements.Add(MapIcon1);
        }

        /**
         * change the tilesource of the mapControl to use openstreetmap
         */
        private void setOsmTileSource() {
            var httpsource = new HttpMapTileDataSource("http://a.tile.openstreetmap.org/{zoomlevel}/{x}/{y}.png");
            var ts = new MapTileSource(httpsource) {
                Layer = MapTileLayer.BackgroundReplacement
            };
            mapControl.Style = MapStyle.None;
            mapControl.TileSources.Add(ts);
        }
        
        private void ZoomIn_Click(object sender, RoutedEventArgs e) {
            mapControl.ZoomLevel += 1;
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e) {
            mapControl.ZoomLevel -= 1;
        }
    }
}
