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
#elif BETA
        public const string BUILDTYPE = "Beta";
#elif DEBUG
        public const string BUILDTYPE = "Debug";
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
            });
        }

    }
}
