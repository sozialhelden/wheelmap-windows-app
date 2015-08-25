using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap_Windows.Utils;
using Wheelmap_Windows.Utils.Eventbus;
using Wheelmap_Windows.Utils.Eventbus.Events;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

namespace Wheelmap_Windows
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private static string TAG = "App";

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            Microsoft.ApplicationInsights.WindowsAppInitializer.InitializeAsync(
                Microsoft.ApplicationInsights.WindowsCollectors.Metadata |
                Microsoft.ApplicationInsights.WindowsCollectors.Session);
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            
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
            ShowMainPage();

            SetUpVoiceCommands();
        }

        protected override void OnActivated(IActivatedEventArgs args) {
            base.OnActivated(args);
            
            if (args.Kind == ActivationKind.VoiceCommand) {
                var commands = args as VoiceCommandActivatedEventArgs;
                SpeechRecognitionResult result = commands.Result;
                string voiceCommandName = result.RulePath[0];
                if (voiceCommandName == "testCommand") {
                    // TODO
                }
                Log.d(TAG, "VoiceCommand: " + voiceCommandName);
                InitMangers();
                InitWindow();
                ShowMainPage();
            }
            Window.Current.Activate();
        }

        private void InitMangers() {
            DataHolder.Init();
            LocationManager.Init();
        }

        private void InitWindow() {
            SetUpTitleBar();
            SystemNavigationManager.GetForCurrentView().BackRequested += (source, args) => GoBack();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons")) {
                Windows.Phone.UI.Input.HardwareButtons.BackPressed += (source, args) => GoBack();
            }
        }
        
        private void ShowMainPage() {
            
            Frame rootFrame = Window.Current.Content as Frame;
            
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null) {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                /*if (e.PreviousExecutionState == ApplicationExecutionState.Terminated) {
                    //TODO: Load state from previously suspended application
                }*/

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null) {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(Source.UI.MainPage)/*, e.Arguments*/);
            }
            // Ensure the current window is active
            Window.Current.Activate();
            
        }

        private async void SetUpVoiceCommands() {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Commands.xml"));
            await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(file);
        }
        
        public void Navigate(Type type, object param = null) {
            Log.d(TAG, "Navigate to " + type);
            Frame rootFrame = Window.Current.Content as Frame;
            rootFrame.Navigate(type, param);
            RefreshBackStatus();
        }

        public void GoBack() {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.Content is BackDelegate) {
                var backhandling = rootFrame.Content as BackDelegate;
                if (backhandling.CanGoBack()) {
                    backhandling.GoBack();
                    RefreshBackStatus();
                    return;
                }
            }
            if (rootFrame.CanGoBack) {
                rootFrame.GoBack();
            }
            RefreshBackStatus();
        }

        public void RefreshBackStatus() {
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
            } else {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
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
