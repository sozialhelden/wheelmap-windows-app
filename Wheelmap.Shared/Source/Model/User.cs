using Newtonsoft.Json;
using Wheelmap.Utils.Eventbus;
using Wheelmap.Utils.Eventbus.Events;
using Wheelmap.Utils.Preferences;

namespace Wheelmap.Model {
    public class User {

        public int id { get; set; }

        public string email { get; set; }

        [JsonProperty(PropertyName = "api_key")]
        public string apiKey { get; set; }

        [JsonProperty(PropertyName = "terms_accepted")]
        public bool termsAccepted { get; set; }

        [JsonProperty(PropertyName = "privacy_Accepted")]
        public bool privacyAccepted { get; set; }
        
        private static User _currentUser;
        public static User CurrentUser {
            get {
                if (_currentUser == null) {
                    _currentUser = Prefs.GetCurrentUser();
                }
                return _currentUser;
            }
            set {

                if (_currentUser != value) {
                    _currentUser = value;
                    BusProvider.DefaultInstance.Post(new UserChangedEvent { User = value });
                }
                Prefs.SetCurrentUser(_currentUser);
            }
        }
    }
}
