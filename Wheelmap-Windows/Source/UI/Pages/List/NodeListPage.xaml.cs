using System.Collections.Generic;
using System.Linq;
using Wheelmap.Utils;
using Wheelmap.Utils.Eventbus;
using Wheelmap.Utils.Eventbus.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Wheelmap.Extensions;
using Wheelmap.UI.Pages.Base;
using Wheelmap.Model;

namespace Wheelmap.Source.UI.Pages.List {

    public sealed partial class NodeListPage : BasePage {

        public override string Title {
            get {
                return isInHelpMode 
                    ? "TITLE_HELP".t().ToUpper() 
                    : "TITLE_LIST".t().ToUpper();
            }
        }

        public object NodesHelper { get; private set; }

        bool isInHelpMode = false;
        private Filter Filter;
        private BulkObservableCollection<Model.Node> mItems = new BulkObservableCollection<Model.Node>();
        SearchBoxHandler searchHandler;


        public NodeListPage() {
            this.InitializeComponent();
            helpHintText.Text = "HELP_HINT".t();
            emptyListTextBlock.Text = "NO_NODES_FOUND".t();
            WindowSizeStates.CurrentStateChanged += (sender, e) => OnStateChanged();
            searchHandler = new SearchBoxHandler(searchBox);
        }
        
        private void OnStateChanged() {
            // show or hide searchBox
            if (WindowSizeStates.CurrentState == STATE_MEDIUM && !isInHelpMode) {
                searchBox.Visibility = Visibility.Visible;
            } else {
                searchBox.Visibility = Visibility.Collapsed;
            } 
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
                isInHelpMode = param.HelpMode;
                if (param.HelpMode) {
                    Filter = new Filter();
                    Filter.FilterdStati.Add(Model.Status.YES);
                    Filter.FilterdStati.Add(Model.Status.LIMITED);
                    Filter.FilterdStati.Add(Model.Status.NO);
                    SetData(Filter.FilterNodes(DataHolder.Instance.Nodes));
                    helpHintTextBorder.Visibility = Visibility.Visible;
                    searchBox.Visibility = Visibility.Collapsed;
                } else {
                    SetData(DataHolder.Instance.FilterdNodes);
                    helpHintTextBorder.Visibility = Visibility.Collapsed;
                    searchBox.Visibility = Visibility.Visible;
                }
            }
            titleTextBlock.Text = Title ?? "";
            OnStateChanged();
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

            var orderedData = Nodes.OrderItemsByDistance(data, LocationManager.Instance?.LastLocationEvent?.Args?.Position?.Coordinate?.Point);
            mItems.CallBatch(() => {
                mItems.Clear();
                mItems.AddAll(orderedData);
            });
            
            if (mItems.Count() <= 0) {
                emptyListTextBlock.Visibility = Visibility.Visible;
            } else {
                emptyListTextBlock.Visibility = Visibility.Collapsed;
            }

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
