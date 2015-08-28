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
}
