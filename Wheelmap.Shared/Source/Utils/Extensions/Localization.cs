using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.Resources.Core;

namespace Wheelmap.Extensions {

    public static class Localization {
        
        static ResourceLoader mResourceLoader = new ResourceLoader();

        public static string t(this string key, string file = null) {
            try { 
                if (file != null) {
                    return ResourceLoader.GetForCurrentView(file).GetString(key);
                }
                return mResourceLoader.GetString(key);
            }catch {
                return key;
            }

        }
        
        public static string t(this string key, ResourceContext context, string file = null) {
            try {
                if (context == null) {
                    return key.t(file);
                }
                if (file != null) {
                    return ResourceManager.Current.MainResourceMap.GetSubtree(file).GetValue(key, context).ValueAsString;
                }
                return ResourceManager.Current.MainResourceMap.GetSubtree("Resources").GetValue(key, context).ValueAsString;
            } catch {
                return key;
            }
        }

    }
}
