using System;
using System.Threading.Tasks;
using Wheelmap.Extensions;
using Wheelmap.Utils.Eventbus;
using Wheelmap.Utils.Eventbus.Events;
using Windows.Devices.Geolocation;
using Windows.System;
using Windows.UI.Xaml.Controls;

namespace Wheelmap.Utils {
    public class LocationManager {

        private static LocationManager _instance;
        public static LocationManager Instance {
            get {
                if (_instance == null) {
                    _instance = new LocationManager();
                }
                return _instance;
            }
        }

        public static void Init() {
            var ignore = Instance;
        }
        
        private Geolocator mGeolocator = new Geolocator();

        public LocationChangedEvent LastLocationEvent;
        public GeolocationAccessStatus AccessStatus;

        public LocationManager() {
            InitializeGeolocation();
            mGeolocator.StatusChanged += Geolocator_StatusChanged;
            mGeolocator.PositionChanged += Geolocator_PositionChanged;
        }

        private async void InitializeGeolocation() {
            AccessStatus = await RequestAccess();
        }

        public async Task<GeolocationAccessStatus> RequestAccess() {
            return await Geolocator.RequestAccessAsync();
        }

        public async Task<bool> ShowLocationSettingsPage() {
           return await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
        }

        public async void ShowNoAccessDialog() {
            AccessStatus = await RequestAccess();
            if (AccessStatus == GeolocationAccessStatus.Allowed) {
                return;
            }

            TextBlock content = new TextBlock {
                Text = "ERROR_NO_LOCTION_ACCESS_DisplayMessage".t(R.File.CORTANA),
                TextAlignment = Windows.UI.Xaml.TextAlignment.Center,
                Margin = new Windows.UI.Xaml.Thickness { Bottom = 10, Left = 10, Top = 10, Right = 10},
                TextWrapping = Windows.UI.Xaml.TextWrapping.WrapWholeWords,
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
            };
            ContentDialog dialog = new ContentDialog() {
                Content = content
            };
            dialog.SecondaryButtonText = "DIALOG_NO_LOCATION_ACCESS_CANCEL".t();
            dialog.PrimaryButtonText = "DIALOG_NO_LOCATION_ACCESS_SETTINGS_BUTTON_TEXT".t();
            dialog.PrimaryButtonClick += (d, _) => {
                ShowLocationSettingsPage().Forget();
            };
            
            await dialog.ShowAsync();
        }

        private void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args) {
            Log.d("NewLocation: " + args?.Position?.Coordinate?.Point?.Position.Latitude+":"+args?.Position?.Coordinate?.Point?.Position.Longitude);

            if (!DataHolder.Instance.HadData) {
                DataHolder.Instance.HadData = true;
                /*var task = new NodesRequest(GeoMath.CalculateBoundingBox(args.Position.Coordinate.Point, 0.8)).Query();
                task.ContinueWith((items) => {
                    DataHolder.Instance.Nodes = items.Result;
                });*/
            }

            LocationChangedEvent e = new LocationChangedEvent() {
                Sender = sender,
                Args = args
            };
            LastLocationEvent = e;
            BusProvider.DefaultInstance.Post(e);
        }

        private void Geolocator_StatusChanged(Geolocator sender, StatusChangedEventArgs args) {
            Log.d("Geolocator Status: " + args.Status.ToString());
        }
    }
}
