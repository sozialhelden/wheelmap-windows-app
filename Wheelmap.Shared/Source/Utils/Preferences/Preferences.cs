using Newtonsoft.Json;
using System;
using Wheelmap.Model;
using Windows.Storage;
using Wheelmap.Extensions;

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

        public static Filter Filter {
            get {
                return localSettings.ReadFromJson<SerializableFilter>(KEY_FILTER)?.ToFilter();
            }
            set {
                localSettings.SaveAsJson(KEY_FILTER, value?.toSerializableFilter());
            }
        }
        
        public static bool FirstAppStart {
            get {
                bool? value = localSettings.Values[KEY_FIRST_APP_START] as bool?;
                return value ?? true;
            }
            set {
                localSettings.Values[KEY_FIRST_APP_START] = value;
            }
        }

        public static User CurrentUser {
            get {
                return localSettings.ReadFromJson<User>(KEY_CURRENTUSER);
            }
            set {
                localSettings.SaveAsJson(KEY_CURRENTUSER, value);
            } 
        }

        public static String InstallId {
            get {
                string id = localSettings.Values[KEY_INSTALL_ID] as string;
                if (id == null) {
                    id = Guid.NewGuid().ToString();
                    localSettings.Values[KEY_INSTALL_ID] = id;
                }
                return id;
            }
        }
        
    }
   
}
