using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace Wheelmap_Windows.Extensions {
    
    public static class Localization {
        
        static ResourceLoader mResourceLoader = new ResourceLoader();

        public static string t(this string key, string file = null) {
            if (file != null) {
                return ResourceLoader.GetForCurrentView(file).GetString(key);
            }
            return mResourceLoader.GetString(key);
        }

    }
}
