using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Model;

namespace Wheelmap_Windows {
    static class BuildConfig {

        public const string API_KEY = "jWeAsb34CJq4yVAryjtc";

        public static string API_KEY_PARAM {
            get {
                // use api token of the user if possible
                if (User.CurrentUser?.apiKey != null) {
                    return "api_key=" + User.CurrentUser?.apiKey;
                }
                return "api_key=" + API_KEY;
            }
        }

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

    }
}
