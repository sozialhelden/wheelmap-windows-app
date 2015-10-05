using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Model;
using Windows.Storage;

namespace Wheelmap {
    
    /**
     * contains every build dependent information
     */
    public static class WMBuildConfig {
        
        private const string SETTINGS_FILE_NAME = "ms-appx:///Resources/Conf/settings.json";

        // API ENDPOINT
#if DEBUG
        public const string API_BASEURL = "http://staging.wheelmap.org";
#else
        public const string API_BASEURL = "http://wheelmap.org";
#endif

        // BUILDTYPE
#if ALPHA
        public const string BUILDTYPE = "Alpha";
        public const string HOCKEY_APP_ID = "ea302e3565c86f05253fc591110b40ae";
#elif BETA
        public const string BUILDTYPE = "Beta";
        public const string HOCKEY_APP_ID = "ea302e3565c86f05253fc591110b40ae";
#elif DEBUG
        public const string BUILDTYPE = "Debug";
        public const string HOCKEY_APP_ID = null;
#else
        public const string BUILDTYPE = "Release";
#endif
        
        /**
         * must be called in the apps constructor
         */
        public static async Task Init() {

            string fileContent;
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Resources/Conf/settings.json"));
            using (StreamReader sRead = new StreamReader(await file.OpenStreamForReadAsync())) {
                fileContent = await sRead.ReadToEndAsync();
            }
            var config = JsonConvert.DeserializeObject<Config>(fileContent);
            config.BUILDTYPE = WMBuildConfig.BUILDTYPE;
            config.HOCKEY_APP_ID = WMBuildConfig.HOCKEY_APP_ID;
            config.API_BASEURL = WMBuildConfig.API_BASEURL;

            BuildConfig.Init(config);
        }

    }
}
