using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Wheelmap.Extensions {
    public static class PreferencesExtensions {

        /// <summary>
        /// trys to read the value stored in {key} as JsonObject 
        /// </summary>
        /// <param name="key">
        /// the unique key for the local settings
        /// </param>
        /// <returns>
        /// the stored value or default
        /// </returns>
        public static T ReadFromJson<T>(this ApplicationDataContainer localSettings, String key, T defaultValue = default(T)) {
            try {
                string json = localSettings.Values[key] as string;
                if (json != null) {
                    return JsonConvert.DeserializeObject<T>(json);
                } else {
                    return defaultValue;
                }
            }
            catch (Exception e) {
                // something goes wrong
                Log.e(e);
                return defaultValue;
            }
        }

        /// <summary>
        /// saves the type as json in localSettings
        /// stores null if the value is null
        /// </summary>
        public static void SaveAsJson<T>(this ApplicationDataContainer localSettings, String key, T value) {
            try {
                if (value == null) {
                    localSettings.Values[key] = null;
                    return;
                }
                var json = JsonConvert.SerializeObject(value);
                localSettings.Values[key] = json;
            }catch(Exception e) {
                Log.e(e);
            }
        }

    }
}
