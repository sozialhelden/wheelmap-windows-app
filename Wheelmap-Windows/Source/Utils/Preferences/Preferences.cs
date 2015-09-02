using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Wheelmap_Windows.Utils.Preferences {
    class ApiPreferences {

        private static ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        
        public static void SetEtag(string url, string etag) {
            localSettings.Values["ETAG_" + url] = etag;
        }

        public static string GetEtag(string url) {
            return localSettings.Values["ETAG_" + url] as string;
        }
        
    }

    public class Prefs {

        private static ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        const string KEY_CURRENTUSER = "CurrentUser";

        public static Model.User GetCurrentUser() {
            string userJson = localSettings.Values[KEY_CURRENTUSER] as string;
            if (userJson != null) {
                return JsonConvert.DeserializeObject<Model.User>(userJson); ;
            } else {
                return null;
            }
        }

        public static void SetCurrentUser(Model.User user) {
            if (user == null) {
                localSettings.Values[KEY_CURRENTUSER] = null;
                return;
            }
            var json = JsonConvert.SerializeObject(user);
            localSettings.Values[KEY_CURRENTUSER] = json;
        }
    }
}
