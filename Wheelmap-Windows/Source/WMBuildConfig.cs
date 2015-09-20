using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Model;

namespace Wheelmap {
    
    /**
     * contains every build dependent information
     */
    public static class WMBuildConfig {

        public const string API_KEY = "jWeAsb34CJq4yVAryjtc";
        
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
        public static void Init() {
            BuildConfig.Init(new Config {
                BUILDTYPE = WMBuildConfig.BUILDTYPE,
                API_KEY = WMBuildConfig.API_KEY,
                API_BASEURL = WMBuildConfig.API_BASEURL,
                HOCKEY_APP_ID = WMBuildConfig.HOCKEY_APP_ID
            });
        }

    }
}
