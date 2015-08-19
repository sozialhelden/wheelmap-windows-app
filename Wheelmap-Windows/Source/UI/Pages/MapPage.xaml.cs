using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap_Windows.Api.Calls;
using Wheelmap_Windows.Model;
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
        public MapPage() {
            this.InitializeComponent();


            // todo load async
            Node[] items = NodeApiClient.GetNodes();

            foreach (Node n in items) {
                Debug.WriteLine("Items: " + n.name);
                AddNewMapIcons(n);
            }

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
