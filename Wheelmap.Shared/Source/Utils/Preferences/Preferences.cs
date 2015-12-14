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
            return readFromJson<Model.User>(KEY_CURRENTUSER);
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
            return readFromJson<Filter>(KEY_FILTER);
        }
        
        public static String GetInstallId() {
            string id = localSettings.Values[KEY_INSTALL_ID] as string;
            if (id == null) {
                id = Guid.NewGuid().ToString();
                localSettings.Values[KEY_INSTALL_ID] = id;
            }
            return id;
        }

        /// <summary>
        /// trys to read the value stored in {key} as JsonObject 
        /// </summary>
        /// <param name="key">
        /// the unique key for the local settings
        /// </param>
        /// <returns>
        /// the stored value or default
        /// </returns>
        private static T readFromJson<T>(String key) {
            try {
                string json = localSettings.Values[key] as string;
                if (json != null) {
                    return JsonConvert.DeserializeObject<T>(json);
                } else {
                    return default(T);
                }
            } catch (Exception e) {
                // something goes wrong
                Log.e(e);
                return default(T);
            }
        }
        
    }
   
}
