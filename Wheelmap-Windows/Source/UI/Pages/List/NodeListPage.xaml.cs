using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap_Windows.Utils;
using Wheelmap_Windows.Utils.Eventbus;
using Wheelmap_Windows.Utils.Eventbus.Events;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Wheelmap_Windows.Extensions;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Windows.Devices.Geolocation;
using Wheelmap_Windows.UI.Pages.Base;

namespace Wheelmap_Windows.Source.UI.Pages.List {
    
    public sealed partial class NodeListPage : BasePage {

        public override string Title {
            get {
                return "TITLE_LIST".t().ToUpper();
            }
        }

        private BulkObservableCollection<Model.Node> mItems = new BulkObservableCollection<Model.Node>();

        public NodeListPage() {
            this.InitializeComponent();    
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            listView.ItemsSource = mItems;
            SetData(DataHolder.Instance.Nodes);
            BusProvider.DefaultInstance.Register(this);
        }
        
        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            base.OnNavigatedFrom(e);
            this.Unregister();
        }
       
        private void Node_Selected(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count() <= 0) {
                return;
            }
            Model.Node n = e.AddedItems[0] as Model.Node;
            SelectedNodeChangedEvent newEvent = new SelectedNodeChangedEvent();
            newEvent.node = n;
            BusProvider.DefaultInstance.Post(newEvent);
        }
        
        private void SetData(ICollection<Model.Node> data) {
            if (data == null) {
                return;
            }

            var orderedData = OrderItemsByDistance(data, LocationManager.Instance?.LastLocationEvent?.Args?.Position?.Coordinate?.Point);
            mItems.CallBatch(() => {
                mItems.Clear();
                mItems.AddAll(orderedData);
            });

        }

        [Subscribe]
        public void OnNewData(NewNodesEvent e) {
            SetData(e?.nodes);
        }

        [Subscribe]
        public void OnLocationChanged(LocationChangedEvent e) => SetData(mItems);

        private ICollection<Model.Node> OrderItemsByDistance(ICollection<Model.Node> nodes, Geopoint location) {
            if (nodes == null || location == null) {
                return nodes;
            }
            var point = Position.From(location);
            var elist = nodes.OrderBy(node => {
                double meters = Haversine.DistanceInMeters(point, new Position() { Latitude = node.lat, Longitude = node.lon });
                // update distance to show on map
                node.Distance = meters;
                return meters;
            });
            var list = elist.ToList();
            return list;
        }

    }
}
