using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap_Windows.Utils {
    public interface BackDelegate {
        bool CanGoBack();
        void GoBack();
    }

    public static class BackDelegates {

        /**
         * ask the application to check the current back button status
         */
        public static void RefreshCanGoBack(this BackDelegate handling) {
            (App.Current as App).RefreshBackStatus();
        }
    }
}
