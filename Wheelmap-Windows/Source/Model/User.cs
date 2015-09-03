using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Utils.Eventbus;
using Wheelmap_Windows.Utils.Eventbus.Events;
using Wheelmap_Windows.Utils.Preferences;

namespace Wheelmap_Windows.Model {
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
