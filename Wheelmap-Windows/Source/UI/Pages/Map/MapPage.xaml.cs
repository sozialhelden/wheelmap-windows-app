using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap_Windows.Api.Calls;
using Wheelmap_Windows.Extensions;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.Source.UI.Pages.Map;
using Wheelmap_Windows.Utils;
using Wheelmap_Windows.Utils.Eventbus;
using Wheelmap_Windows.Utils.Eventbus.Events;
using Wheelmap_Windows.Utils.Extensions;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
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

        private static string TAG = "MapPage";

        private const byte ZOOMLEVEL_MIN = 16;
        private const byte ZOOMLEVEL_MAX = 14;

        private Geopoint DEBUG_POSITION = new Geopoint(new BasicGeoposition() { Latitude = 52.5139845511952, Longitude = 13.3907276745181, Altitude = 0});
        private const int MAP_ZOOM_DEFAULT = 18; // Zoon 1 is world view

        int oldZoomLevel = 1;
        Geopoint lastRequestedPosition;

        Dictionary<MapElement, Model.Node> nodeMapIcons = new Dictionary<MapElement, Model.Node>();

        public MapPage() {
            this.InitializeComponent();

            //mapControl.SetOsmTileSource();
            mapControl.CenterChanged += MapControl_CenterChanged;
            mapControl.ZoomLevelChanged += MapControl_ZoomLevelChanged;
            mapControl.MapElementClick += MapControl_MapElementClick;

            mapControl.ZoomLevel = MAP_ZOOM_DEFAULT;
            mapControl.Center = DEBUG_POSITION;
            BusProvider.DefaultInstance.Register(this);

            new MyLocationOverlay(mapControl);

        }
        
        private void MapControl_MapElementClick(MapControl sender, MapElementClickEventArgs args) {
            Debug.WriteLine("MapElementClick: "+ sender + "-"+ args.MapElements.First());
            
            if (args?.MapElements?.Count > 0) {
                var node = nodeMapIcons[args.MapElements.First()];
                SelectedNodeChangedEvent e = new SelectedNodeChangedEvent();
                e.node = node;
                BusProvider.DefaultInstance.Post(e);
            }

        }

        /**
         * called when the maps ZoomLevel has changed
         * request new data if needed
         */
        private void MapControl_ZoomLevelChanged(MapControl sender, object args) {
            Debug.WriteLine("ZoomLevel: " + mapControl.ZoomLevel);
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

            oldZoomLevel = zoomLevel;
            RequestUpdate();
        }
        
        /**
         * called when the maps center has changed
         * request new data if map moves more than 30% from the last position depending on the maps size
         */
        private void MapControl_CenterChanged(MapControl sender, object args) {
            var bbox = mapControl.GetBoundingBox();
            if (bbox == null) {
                return;
            }
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
            
            var bbox = mapControl.GetBoundingBox();
            if (bbox == null) {
                return;
            }

            var task = new NodesRequest(bbox).Query();
            task.ContinueWith((items) => {
                var e = new NewNodesEvent();
                e.nodes = items.Result;
                BusProvider.DefaultInstance.Post(e);
            });
        }
        
        [Subscribe]
        public void OnNewData(NewNodesEvent e) {
            nodeMapIcons.Clear();
            mapControl.MapElements.Clear();
            foreach (Model.Node n in e.nodes) {
                AddNewMapIcons(n);
            }
        }
        
        private void AddNewMapIcons(Model.Node node) {
            MapIcon mapIcon = new MapIcon();
            mapIcon.Location = new Geopoint(new BasicGeoposition() {
                Latitude = node.lat,
                Longitude = node.lon
            });
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon.Title = node.name == null ? "" : node.name;
            
            mapControl.MapElements.Add(mapIcon);
            nodeMapIcons.Add(mapIcon, node);
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e) {
            mapControl.ZoomLevel += 1;
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e) {
            mapControl.ZoomLevel -= 1;
        }

        private void OnMyPosition_Click(object sender, RoutedEventArgs e) {
            var point = LocationManager.Instance?.LastLocationEvent?.Args?.Position?.Coordinate?.Point;
            if (point != null) {
                mapControl.Center = point; 
            }
        }

        private void OnCompass_Click(object sender, RoutedEventArgs e) {
            mapControl.TryRotateToAsync(0).AsTask().forget();
        }
        
    }

}
