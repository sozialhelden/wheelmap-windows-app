using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;

namespace Wheelmap.Extensions {
    
    public static class Localization {
        
        static ResourceLoader mResourceLoader = new ResourceLoader();

        public static string t(this string key, string file = null) {
            if (file != null) {
                return ResourceLoader.GetForCurrentView(file).GetString(key);
            }
            return mResourceLoader.GetString(key);
        }
        
        public static string t(this string key, ResourceContext context, string file = null) {
            if (context == null) {
                return key.t(file);
            }
            if (file != null) {
                return ResourceManager.Current.MainResourceMap.GetSubtree(file).GetValue(key, context).ValueAsString;
            }
            return ResourceManager.Current.MainResourceMap.GetSubtree("Resources").GetValue(key, context).ValueAsString;
        }

    }
}
