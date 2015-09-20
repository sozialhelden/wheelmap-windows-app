using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Api.Calls;
using Wheelmap.Extensions;
using Wheelmap.Utils.Eventbus;
using Wheelmap.Utils.Eventbus.Events;
using Windows.Devices.Geolocation;
using Windows.System;

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

        private static string TAG = "LocationManager";

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

        private void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args) {
            Log.d(TAG, "NewLocation: " + args?.Position?.Coordinate?.Point?.Position.Latitude+":"+args?.Position?.Coordinate?.Point?.Position.Longitude);

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
            Log.d(TAG, "Geolocator Status: " + args.Status.ToString());
        }
    }
}
