using HockeyApp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Wheelmap.Cortana;
using Wheelmap.Extensions;
using Wheelmap.Model;
using Wheelmap.Source.UI;
using Wheelmap.Source.UI.Pages.Splashscreen;
using Wheelmap.Utils;
using Wheelmap.Utils.Eventbus;
using Wheelmap.Utils.Eventbus.Events;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.SpeechRecognition;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {

        public static App Instance { private set; get; }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            // this must be done before anything else can happen
            // The BuildConfigs may contain important informations for the appstart
            WMBuildConfig.Init().Wait();

            Instance = this;
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            if (BuildConfig.HOCKEY_APP_ID?.Length > 0) {
                HockeyClient.Current.Configure(BuildConfig.HOCKEY_APP_ID);
            }

        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Debug.WriteLine("BuildType: " + BuildConfig.BUILDTYPE);

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                //this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            
            InitMangers();
            InitWindow();
            ShowMainPage(e);

            CortanaManager.RegisterCommands();

            SendCrashedAsync().Forget();
        }

        private async Task SendCrashedAsync() {
            if (BuildConfig.HOCKEY_APP_ID?.Length > 0) {
                try {
                    await HockeyClient.Current.SendCrashesAsync();
                } catch (Exception ex) {
                    Log.e("Crash in SendCrashesAsync from HockeyApp Sdk");
                    Log.e(ex);
                    // there is an bug in the HockeyApp SDK that causes the crash in some situations
                    // http://stackoverflow.com/questions/32436976/hockeyapp-crashes-on-back-button-press-on-windows-phone/32444472#32444472
                }
            }
        }

        protected override void OnActivated(IActivatedEventArgs args) {
            base.OnActivated(args);

            InitMangers();
            InitWindow();
            switch (args.Kind) {
                case ActivationKind.VoiceCommand:
                    CortanaManager.OnActivated(args as VoiceCommandActivatedEventArgs);
                    Window.Current.Activate();
                    break;
                case ActivationKind.Protocol:
                    var a = args as ProtocolActivatedEventArgs;
                    WwwFormUrlDecoder decoder = new WwwFormUrlDecoder(a.Uri.Query);
                    var wheelmapParams = WheelmapParams.FromString(decoder.GetFirstValueByName("LaunchContext"));
                    ShowMainPage(args, wheelmapParams);
                    break;
                default:
                    ShowMainPage(args);
                    break;
            }
            
        }

        public void InitMangers() {
            var _ = Database.Instance;
            DataHolder.Init();
            LocationManager.Init();
        }

        public void InitWindow() {
            SetUpTitleBar();
            SystemNavigationManager.GetForCurrentView().BackRequested -= App_BackRequested;
            SystemNavigationManager.GetForCurrentView().BackRequested += App_BackRequested;
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")) {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            }

            // set min dimensions for window
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(320, 500));

            // prevent app from rotating to landscale when running on a phone
            if (Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile") {
                DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            }

        }

        private void App_BackRequested(object sender, BackRequestedEventArgs e) {
            e.Handled = GoBack();
        }

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e) {
            e.Handled = GoBack();
        }

        public void ShowMainPage(IActivatedEventArgs args, object paramForMainPage = null) {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null) {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                //  Display an extended splash screen if app was not previously running.
                if (args.PreviousExecutionState != ApplicationExecutionState.Running) {
                    bool loadState = (args.PreviousExecutionState == ApplicationExecutionState.Terminated);
                    ExtendedSplashPage extendedSplash = new ExtendedSplashPage(args.SplashScreen, loadState);
                    extendedSplash.paramForMainPage = paramForMainPage;
                    rootFrame.Content = extendedSplash;
                    Window.Current.Content = rootFrame;
                }
            }

            if (rootFrame.Content == null || paramForMainPage != null) {

                if (rootFrame.Content is MainPage) {
                    (rootFrame.Content as MainPage).OnNewParams(paramForMainPage);
                } else {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), paramForMainPage);
                }

            }
            // Ensure the current window is active
            Window.Current.Activate();

        }
        
        public void Navigate(Type type, object param = null) {
            Log.d("Navigate to " + type);
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(type, param);
            RefreshBackStatus();
        }

        /**
         * returns true if back was handled
         */
        public bool GoBack() {
            bool handled = false;
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.Content is BackDelegate) {
                var backhandling = rootFrame.Content as BackDelegate;
                if (backhandling.CanGoBack()) {
                    backhandling.GoBack();
                    handled = true;
                    RefreshBackStatus();
                    return handled;
                }
            }
            if (rootFrame.CanGoBack) {
                rootFrame.GoBack();
                handled = true;
            }
            RefreshBackStatus();
            return handled;
        }
        
        /**
         * returns true if back button is shown
         */
        public bool RefreshBackStatus() {
            bool canGoBack = false;
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.Content is BackDelegate) {
                var backhandling = rootFrame.Content as BackDelegate;
                if (backhandling.CanGoBack()) {
                    canGoBack = true;
                }
            }
            if (rootFrame.CanGoBack) {
                canGoBack = true;
            }

            if (canGoBack) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                return true;
            } else {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                return false;
            }

        }

        /**
         * change the titlebars background and foreground color
         */
        private void SetUpTitleBar() {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            Color bgColor = Color.FromArgb(0xFF, 0x2c, 0x3b, 0x49);

            titleBar.BackgroundColor = bgColor;
            titleBar.ForegroundColor = Colors.White;
            titleBar.InactiveBackgroundColor = bgColor;
            titleBar.ButtonBackgroundColor = bgColor;
            titleBar.ButtonForegroundColor = Colors.White;
            titleBar.ButtonInactiveBackgroundColor = bgColor;

            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = bgColor;
                statusBar.BackgroundOpacity = 1;
                statusBar.ForegroundColor = Colors.White;
                DataHolder.Instance.PropertyChanged -= DataHolder_PropertyChanged;
                DataHolder.Instance.PropertyChanged += DataHolder_PropertyChanged;
            }
        }

        private void DataHolder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(DataHolder.Instance.IsRequestRunning)) {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                    if (DataHolder.Instance.IsRequestRunning) {
                        statusBar.ProgressIndicator.ShowAsync().Forget();
                    } else {
                        statusBar.ProgressIndicator.HideAsync().Forget();
                    }
                }).Forget();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
