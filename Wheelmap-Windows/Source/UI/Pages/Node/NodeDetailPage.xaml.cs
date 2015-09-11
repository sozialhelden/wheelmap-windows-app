using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wheelmap_Windows.Api.Calls;
using Wheelmap_Windows.Api.Model;
using Wheelmap_Windows.Extensions;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.Source.UI.Pages.ImagesDetail;
using Wheelmap_Windows.Source.UI.Pages.Profile;
using Wheelmap_Windows.Source.UI.Pages.Status;
using Wheelmap_Windows.UI.Pages.Base;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap_Windows.Source.UI.Pages.Node {

    public sealed partial class NodeDetailPage : BasePage {

        public override string Title {
            get {
                return CurrentNode.name;
            }
        }

        public Model.Node CurrentNode;
        public List<Photo> mPhotos;



        public NodeDetailPage() {
            this.InitializeComponent();
            scrollViewer.HideVerticalScrollBarsIfContentFits();
            //mapControl.SetOsmTileSource();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            SetNode(e.Parameter as Model.Node);
        }

        public void SetNode(Model.Node n) {
            CurrentNode = Nodes.QueryByWmId(n.wm_id);

            InitTextFields(CurrentNode);
            InitImages(CurrentNode);
            InitMapControl(CurrentNode);
            InitStatus(CurrentNode);

        }

        private void InitTextFields(Model.Node n) {
            nameBlock.Text = n.name ?? "";

            try {
                phoneButton.NavigateUri = new Uri("tel:" + n.phone);
                phoneButton.Content = n.phone;
                phoneButton.Visibility = Visibility.Visible;
            }
            catch {
                phoneButton.Visibility = Visibility.Collapsed;
            }

            plzBlock.Text = n.postcode ?? "";
            streetBlock.Text = n.street ?? "";
            try {
                websideButton.NavigateUri = new Uri(n.website);
                websideButton.Content = n.website;
                websideButton.Visibility = Visibility.Visible;
            }
            catch {
                websideButton.Visibility = Visibility.Collapsed;
            }

            categoryNameTextBlock.Text = n.category?.localizedName ?? "";
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
            new PhotosRequest(node).Execute().ContinueWithOnDispatcher(Dispatcher, (photos) => {
                if (CurrentNode != node) {
                    return;
                }
                mPhotos = photos.Result;
                listView.Items.Clear();
                Log.d(this, "Photos: " + photos.Result.Count());
                listView.Items.Add(new AddNewPhotoPhoto());
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

            if (listView.SelectedIndex == 0) {
                uploadNewPhoto().Forget();
                return;
            }

            var args = new ImagesDetailArguments() {
                photos = mPhotos,
                // -1 for the first choose a photo element
                selectedItem = listView.SelectedIndex - 1
            };

            // clear selection
            listView.SelectedIndex = -1;

            if (Window.Current.Content is Frame) {
                var page = Window.Current.Content as Frame;
                if (page.Content is MainPage) {
                    (page.Content as MainPage).NavigateSecondPage(typeof(ImagesDetailPage), args);
                    return;
                }
            }

            if (App.Current is App) {
                (App.Current as App).Navigate(typeof(ImagesDetailPage), args);
                return;
            }

        }

        private async Task uploadNewPhoto() {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            // Launch file open picker and caller app is suspended and may be terminated if required 
            StorageFile file = await openPicker.PickSingleFileAsync();
            var result = await new PhotoUploadTask(CurrentNode, file).Execute();
            Log.d(this, result.IsOk);
        }

        private void Edit_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            if (User.CurrentUser != null) {
                ShowOnDetailFrame(typeof(NodeEditPage), CurrentNode);
            } else {
                // show login page when user not already logged in
                ShowOnDetailFrame(typeof(LoginPage));
            }
        }

        private void status_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            ChangeStatusDialogPage.ShowInDialog(new ChangeStatusDialogPageArgs { status = Stati.From(CurrentNode.wheelchairStatus), isInWcMode = false }, (status) => {
                var node = CreateStateDirtyNode();
                node.wheelchairStatus = status.ToApiString();
                Nodes.Save(node);
                SetNode(node);
                new RetryRequest<NodeEditResponse>(new NodeStateEditRequest(node)).Execute().Forget();
            });
        }

        private void statusToilet_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            ChangeStatusDialogPage.ShowInDialog(new ChangeStatusDialogPageArgs { status = Stati.From(CurrentNode.wheelchairToiletStatus), isInWcMode = true }, (status) => {
                var node = CreateStateDirtyNode();
                node.wheelchairToiletStatus = status.ToApiString();
                Nodes.Save(node);
                SetNode(node);
                new RetryRequest<NodeEditResponse>(new NodeEditToiletStatusRequest(node)).Execute().Forget();
            });
        }

        private Model.Node CreateStateDirtyNode() {
            var node = CurrentNode.CreateCopyIfNeeded();
            if (node.DirtyState != DirtyState.DIRTY_ALL) {
                node.DirtyState = DirtyState.DIRTY_STATE;
            }
            return node;
        }

    }

    class AddNewPhotoPhoto {

        public Model.Image Thumb {
            get {
                return new Model.Image {
                    url = "ms-appx:///Assets/Images/ic_camera.png"
                };
            }
        }

    }

}
