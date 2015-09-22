using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap.Api.Calls;
using Wheelmap.Extensions;
using Wheelmap.Model;
using Wheelmap.Source.UI.Pages.Map;
using Wheelmap.Source.UI.Pages.Node;
using Wheelmap.UI.Pages.Base;
using Wheelmap.Utils;
using Wheelmap.Utils.Eventbus;
using Wheelmap.Utils.Eventbus.Events;
using Wheelmap.Utils.Extensions;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap.Source.UI.Pages {

    public sealed partial class MapPage : BasePage {
        
        private static string TAG = "MapPage";
        
        public override string Title {
            get {
                return "TITLE_MAP".t().ToUpper();
            }
        }
        
        private const byte ZOOMLEVEL_MAX = 16;

        private const byte ZOOMLEVEL_MIN = 14;


        // Center of Germany
        public static Geopoint DEFAULT_POSITION = new Geopoint(new BasicGeoposition() { Latitude = 52.047783, Longitude = 13.0546663, Altitude = 0 });

        // Zoom 1 is world view
        // on 6.5 you can see Germany
        public const double MAP_ZOOM_DEFAULT = 6.5; 

        int oldZoomLevel = 1;
        Geopoint lastRequestedPosition;
        MyLocationOverlay myLocationOverlay;

        BiDirectionalMap<Model.Node, MapElement> nodeElementMap = new BiDirectionalMap<Model.Node, MapElement>();
        SearchBoxHandler searchHandler;

        public MapPage() {
            this.InitializeComponent();

            //mapControl.SetOsmTileSource();
            mapControl.CenterChanged += MapControl_CenterChanged;
            mapControl.ZoomLevelChanged += MapControl_ZoomLevelChanged;
            mapControl.MapElementClick += MapControl_MapElementClick;

            mapControl.ZoomLevel = MAP_ZOOM_DEFAULT;
            mapControl.Center = DEFAULT_POSITION;

            myLocationOverlay = new MyLocationOverlay(mapControl);
            searchHandler = new SearchBoxHandler(searchBox);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            BusProvider.DefaultInstance.Register(this);
        }

        private void MapControl_MapElementClick(MapControl sender, MapElementClickEventArgs args) {
            
            if (args?.MapElements?.Count > 0) {
                var node = nodeElementMap.Get(args.MapElements.First());
                SelectedNodeChangedEvent e = new SelectedNodeChangedEvent() {
                    node = node,
                    sender = this,
                };
                BusProvider.DefaultInstance.Post(e);
            }

        }

        /**
         * called when the maps ZoomLevel has changed
         * request new data if needed
         */
        private void MapControl_ZoomLevelChanged(MapControl sender, object args) {
            Log.d(this, "ZoomLevel: " + mapControl.ZoomLevel);
            var zoomLevel = (int)mapControl.ZoomLevel;
            bool isZoomedEnough = true;

            // show or hide hint to zoom in
            if (zoomLevel <= ZOOMLEVEL_MIN) {
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

            if (mapControl.ZoomLevel < ZOOMLEVEL_MAX) {
                return;
            }

            lastRequestedPosition = center;
            RequestUpdate();
        }

        private void RequestUpdate() {

            Log.v(this, "RequestUpdate!!!");

            var bbox = mapControl.GetBoundingBox();
            if (bbox == null) {
                return;
            }

            if (DataHolder.Instance.QueryString?.Length >= 3) {
                // do not query new nodes when search is active
                return;
            }

            var task = new NodesRequest(bbox).Execute();
            task.ContinueWith((items) => {
                DataHolder.Instance.Nodes = items.Result;
            });
        }

        [Subscribe]
        public void OnNewData(NewNodesEvent e) {
            if (!e.RefreshAll) {
                // avoid flickering of the MapIcons by Remove or Add only the Elements which are needed

                // collection of all mapElemements which should be removed
                ICollection<MapElement> elementToRemove = new List<MapElement>(mapControl.MapElements);

                foreach (Model.Node n in e.nodes) {
                    // check if node already exists on the map
                    if (nodeElementMap.Contains(n)) {
                        // this element should not be removed
                        elementToRemove.Remove(nodeElementMap.Get(n));
                    } else {
                        // create new element
                        AddNewMapIcons(n);
                    }
                }

                // remove all outdated mapelements
                foreach (MapElement element in elementToRemove) {
                    mapControl.MapElements.Remove(element);
                    nodeElementMap.Remove(element);
                }
                elementToRemove.Clear();
            } else {
                nodeElementMap.Clear();
                mapControl.MapElements.Clear();
                // refresh all
                foreach (Model.Node n in e.nodes) {
                    AddNewMapIcons(n);
                }
            }
        }
        
        private void AddNewMapIcons(Model.Node node) {

            var geopoint = new Geopoint(new BasicGeoposition() {
                Latitude = node.lat,
                Longitude = node.lon
            });

            MapIcon mapIcon = new MapIcon();
            mapIcon.Location = geopoint;
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon.Title = node.name == null ? "" : node.name;
            
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(node.MapIconFileUri);
            mapControl.MapElements.Add(mapIcon);
            nodeElementMap.Add(node, mapIcon);
            
        }

        private void Marker_NodeClicked(object sender, Model.Node node) {
            SelectedNodeChangedEvent e = new SelectedNodeChangedEvent();
            e.node = node;
            BusProvider.DefaultInstance.Post(e);
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e) {
            mapControl.ZoomLevel += 1;
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e) {
            mapControl.ZoomLevel -= 1;
        }

        public void OnMyPosition_Click(object sender, RoutedEventArgs e) {
            var point = LocationManager.Instance?.LastLocationEvent?.Args?.Position?.Coordinate?.Point;
            if (point != null) {
                mapControl.Center = point;
                mapControl.ZoomLevel = 17;
            }
        }

        private void OnCompass_Click(object sender, RoutedEventArgs e) {
            mapControl.TryRotateToAsync(0).AsTask().Forget();
        }

        private void addNewNodeButton_Tapped(object sender, TappedRoutedEventArgs e) {
            ShowOnDetailFrame(typeof(NodeEditPage));
        }

        [Subscribe]
        public void OnSelectedNodeChanged(SelectedNodeChangedEvent e) {
            if (e.sender == this) {
                return;
            }
            mapControl.ZoomLevel = 19;
            mapControl.Center = new Geopoint(new BasicGeoposition {
                Latitude = e.node.lat,
                Longitude = e.node.lon,
            });
        }
    }

}