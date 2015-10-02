namespace Wheelmap.Utils {

    public static class BackDelegates {

        /**
         * ask the application to check the current back button status
         */
        public static void RefreshCanGoBack(this BackDelegate handling) {
            (App.Current as App).RefreshBackStatus();
        }
    }
}
