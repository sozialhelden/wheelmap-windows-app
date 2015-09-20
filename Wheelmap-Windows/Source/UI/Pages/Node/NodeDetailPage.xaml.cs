using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Wheelmap.Api.Calls;
using Wheelmap.Api.Model;
using Wheelmap.Extensions;
using Wheelmap.Model;
using Wheelmap.Source.UI.Controls;
using Wheelmap.Source.UI.Pages.ImagesDetail;
using Wheelmap.Source.UI.Pages.Profile;
using Wheelmap.Source.UI.Pages.Status;
using Wheelmap.UI.Pages.Base;
using Windows.ApplicationModel.DataTransfer;
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

namespace Wheelmap.Source.UI.Pages.Node {

    public sealed partial class NodeDetailPage : BasePage {

        public override string Title {
            get {
                return CurrentNode.name;
            }
        }

        public Model.Node CurrentNode;
        public List<Photo> mPhotos;
        private DataTransferManager m_dTransferMgr;


        public NodeDetailPage() {
            this.InitializeComponent();
            scrollViewer.HideVerticalScrollBarsIfContentFits();
            //mapControl.SetOsmTileSource();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            SetNode(e.Parameter as Model.Node);
            m_dTransferMgr = DataTransferManager.GetForCurrentView();
            m_dTransferMgr.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);
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
            streetBlock.Text = (n.street ?? "") + " " +(n.housenumber ?? "");
            try {
                websideButton.NavigateUri = new Uri(n.website);
                websideButton.Content = n.website;
                websideButton.Visibility = Visibility.Visible;
            }
            catch {
                websideButton.Visibility = Visibility.Collapsed;
            }

            var nodeTypeName = n.nodeType?.localizedName;
            nodeTypeName = nodeTypeName == null ? "" : $" ({nodeTypeName})";
            categoryNameTextBlock.Text = (n.category?.localizedName ?? "") + nodeTypeName;
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


            var dialog = new ProgressDialog();
            dialog.ShowAsync().Forget();
            var result = await new PhotoUploadTask(CurrentNode, file).Execute();
            dialog.Close();
            Log.d(this, result.IsOk);
            SetNode(CurrentNode);
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

        private void Navigation_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            var uriFormat = "ms-drive-to:?destination.latitude={0}&destination.longitude={1}&destination.name={2}";
            var uri = String.Format(uriFormat,
                CurrentNode.lat.ToString(CultureInfo.InvariantCulture),
                CurrentNode.lon.ToString(CultureInfo.InvariantCulture),
                Uri.EscapeDataString(CurrentNode.name)
            );
             Windows.System.Launcher.LaunchUriAsync(new Uri(uri)).Forget();
        }

        private void Share_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            DataTransferManager.ShowShareUI();
        }

        private void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e) {
            DataRequest request = e.Request;
            request.Data.Properties.Title = CurrentNode.name;
            request.Data.Properties.Description = CurrentNode.name;
            var url = BuildConfig.API_BASEURL + String.Format(ApiConstants.NODES_DETAILS, CurrentNode.wm_id);
            request.Data.SetText(CurrentNode.name + ", "+ url);
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
