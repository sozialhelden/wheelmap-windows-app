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
using Windows.UI.Xaml.Media.Animation;
using Wheelmap_Windows.Model;

namespace Wheelmap_Windows.Source.UI.Pages.List {

    public sealed partial class NodeListPage : BasePage {

        public override string Title {
            get {
                return "TITLE_LIST".t().ToUpper();
            }
        }

        private Filter Filter;
        private BulkObservableCollection<Model.Node> mItems = new BulkObservableCollection<Model.Node>();

        public NodeListPage() {
            this.InitializeComponent();
            helpHintText.Text = "HELP_HINT".t();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            listView.ItemsSource = mItems;
            BusProvider.DefaultInstance.Register(this);
        }

        public override void OnNewParams(object args) {
            base.OnNewParams(args);
            if (args == null) {
                SetData(DataHolder.Instance.FilterdNodes);
                helpHintTextBorder.Visibility = Visibility.Collapsed;
            } else {
                var param = args as NodeListPageArgs;
                if (param.HelpMode) {
                    Filter = new Filter();
                    Filter.FilterdStati.Add(Model.Status.YES);
                    Filter.FilterdStati.Add(Model.Status.LIMITED);
                    Filter.FilterdStati.Add(Model.Status.NO);
                    SetData(Filter.FilterNodes(DataHolder.Instance.Nodes));
                    helpHintTextBorder.Visibility = Visibility.Visible;
                } else {
                    SetData(DataHolder.Instance.FilterdNodes);
                    helpHintTextBorder.Visibility = Visibility.Collapsed;
                }
            }
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
            if (Filter != null) {
                SetData(Filter.FilterNodes(DataHolder.Instance.Nodes));
            } else {
                SetData(e?.nodes);
            } 
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

    public class NodeListPageArgs {

        public bool HelpMode = false;

        public override bool Equals(object obj) {
            if (!(obj is NodeListPageArgs)) {
                return false;
            }
            return HelpMode == (obj as NodeListPageArgs).HelpMode;
        }

        public override int GetHashCode() {
            return HelpMode ? 0 : 1;
        }

    }
}
