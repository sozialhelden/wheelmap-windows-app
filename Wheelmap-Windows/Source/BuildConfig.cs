using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap_Windows.Source {
    static class BuildConfig {
        
        // API ENDPOINT
#if DEBUG
        public const string API_BASEURL = "staging.wheelmap.org";
#else
        public const string API_BASEURL = "wheelmap.org";
#endif

        // BUILDTYPE
#if ALPHA
        public const string BUILDTYPE = "Alpha";
#elif BETA
        public const string BUILDTYPE = "Beta";
#elif DEBUG
        public const string BUILDTYPE = "Debug";
#else
        public const string BUILDTYPE = "";
#endif

    }
}
