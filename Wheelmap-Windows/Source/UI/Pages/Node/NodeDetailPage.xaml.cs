using System;
using System.Collections.Generic;
using System.Linq;
using Wheelmap_Windows.Api.Calls;
using Wheelmap_Windows.Extensions;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.Source.Api.Model;
using Wheelmap_Windows.Source.UI.Pages.ImagesDetail;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap_Windows.Source.UI.Pages.Node {

    public sealed partial class NodeDetailPage : Page {

        public Model.Node CurrentNode;
        public List<Photo> mPhotos;

        public NodeDetailPage() {
            this.InitializeComponent();
            //mapControl.SetOsmTileSource();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            SetNode(e.Parameter as Model.Node);
        }

        public void SetNode(Model.Node n) {
            CurrentNode = n;

            InitTextFields(n);
            InitImages(n);
            InitMapControl(n);
            InitStatus(n);

        }

        private void InitTextFields(Model.Node n) {
            nameBlock.Text = n.name ?? "";

            try {
                phoneButton.NavigateUri = new Uri("tel:" + n.phone);
                phoneButton.Content = n.phone;
                phoneButton.Visibility = Visibility.Visible;
            } catch {
                phoneButton.Visibility = Visibility.Collapsed;
            }

            plzBlock.Text = n.postcode ?? "";
            streetBlock.Text = n.street ?? "";
            try {
                websideButton.NavigateUri = new Uri(n.website);
                websideButton.Content = n.website;
                websideButton.Visibility = Visibility.Visible;
            } catch {
                websideButton.Visibility = Visibility.Collapsed;
            }

            categoryNameTextBlock.Text = n.category.localizedName;
            distanceTextBlock.Text = n.DistanceString;

        }

        private void InitMapControl(Model.Node node) {

            mapControl.MapElements.Clear();

            var point = new Geopoint(new BasicGeoposition() {
                Latitude = node.lat,
                Longitude = node.lon
            });

            mapControl.ZoomLevel = 17;
            mapControl.Center = point;

            MapIcon mapIcon = new MapIcon();
            mapIcon.Location = point;
            mapIcon.Image = RandomAccessStreamReference.CreateFromUri(node.MapIconFileUri);
            mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
            mapIcon.Title = node.name == null ? "" : node.name;
            mapControl.MapElements.Add(mapIcon);
            
        }

        private void InitStatus(Model.Node node) {
            var status = Stati.From(node.wheelchairStatus);
            statusBgBorder.Background = new SolidColorBrush(status.GetColor());
            statusTextBlock.Text = status.GetLocalizedMessage();
            statusImage.Source = new BitmapImage(new
                          Uri(status.GetImage(), UriKind.RelativeOrAbsolute));

            var toiletStatus = Stati.From(node.wheelchairToiletStatus);
            statusToiletBgBorder.Background = new SolidColorBrush(toiletStatus.GetColor());
            statusToiletTextBlock.Text = toiletStatus.GetLocalizedToiletMessage();
            statusToiletImage.Source = new BitmapImage(new
                          Uri(toiletStatus.GetImage(), UriKind.RelativeOrAbsolute));

        }

        private void InitImages(Model.Node node) {
            listView.Items.Clear();
            new PhotosRequest(node).Query().ContinueWithOnMainThread((photos) => {
                if (CurrentNode != node) {
                    return;
                }
                mPhotos = photos.Result;
                listView.Items.Clear();
                Log.d(this, "Photos: " + photos.Result.Count());
                foreach (Photo p in photos.Result) {
                    Log.d(this, p.GetThumb());
                    listView.Items.Add(p);
                }
                
            });
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e) {

            // check if items are selected
            if (e.AddedItems.Count() <= 0) {
                return;
            }

            // clear selection
            listView.SelectedIndex = -1;

            if (Window.Current.Content is Frame) {
                var page = Window.Current.Content as Frame;
                if (page.Content is MainPage) {
                    (page.Content as MainPage).NavigateSecondPage(typeof(ImagesDetailPage), mPhotos);
                    return;
                }
            }

            if (App.Current is App) {
                (App.Current as App).Navigate(typeof(ImagesDetailPage), mPhotos);
                return;
            }
            
        }
    }
    
}
