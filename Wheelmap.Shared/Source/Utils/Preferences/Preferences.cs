using Newtonsoft.Json;
using System;
using Wheelmap.Model;
using Windows.Storage;

namespace Wheelmap.Utils.Preferences {
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

        private static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        const string KEY_CURRENTUSER = "CurrentUser";
        const string KEY_FILTER = "Filter";
        const string KEY_INSTALL_ID = "KEY_INSTALL_ID";
        const string KEY_FIRST_APP_START = "KEY_FIRST_APP_START";

        public static bool FirstAppStart {
            get {
                bool? value = localSettings.Values[KEY_FIRST_APP_START] as bool?;
                return value ?? true;
            }
            set {
                localSettings.Values[KEY_FIRST_APP_START] = value;
            }
        }

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

        public static void SaveFilter(Filter filter) {
            if (filter == null) {
                localSettings.Values[KEY_FILTER] = null;
            }
            var json = JsonConvert.SerializeObject(filter);
            localSettings.Values[KEY_FILTER] = json;
        }
        
        public static Filter RestoreFilter() {
            string json = localSettings.Values[KEY_FILTER] as string;
            if (json != null) {
                return JsonConvert.DeserializeObject<Filter>(json); ;
            } else {
                return null;
            }
        }
        
        public static String GetInstallId() {
            string id = localSettings.Values[KEY_INSTALL_ID] as string;
            if (id == null) {
                id = Guid.NewGuid().ToString();
                localSettings.Values[KEY_INSTALL_ID] = id;
            }
            return id;
        }
    }
}
