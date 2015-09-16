using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Wheelmap.Utils.Extensions;
using Wheelmap.Extensions;
using Wheelmap.UI.Pages.Base;

namespace Wheelmap.Source.UI.Pages.Splashscreen {

    public sealed partial class ExtendedSplashPage : BasePage {

        internal Rect splashImageRect; // Rect to store splash screen image coordinates.
        private SplashScreen splash; // Variable to hold the splash screen object.
        internal bool dismissed = false; // Variable to track splash screen dismissal status.
        internal Frame rootFrame;

        public ExtendedSplashPage(SplashScreen splashscreen, bool loadState) {
            this.InitializeComponent();
            // Listen for window resize events to reposition the extended splash screen image accordingly.
            // This ensures that the extended splash screen formats properly in response to window resizing.
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(ExtendedSplash_OnResize);

            splash = splashscreen;
            if (splash != null) {
                // Register an event handler to be executed when the splash screen has been dismissed.
                splash.Dismissed += new TypedEventHandler<SplashScreen, Object>(DismissedEventHandler);

                // Retrieve the window coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();

                // If applicable, include a method for positioning a progress control.
                PositionRing();
            }

            // Create a Frame to act as the navigation context 
            rootFrame = new Frame();

            // Restore the saved session state if necessary
            RestoreStateAsync(loadState);

        }

        async void RestoreStateAsync(bool loadState) {
            LoadData().ContinueWithOnDispatcher(Dispatcher, (task) => {
                DismissExtendedSplash();
            });
        }

        // Position the extended splash screen image in the same location as the system splash screen image.
        void PositionImage() {
            extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
            extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
            extendedSplashImage.Height = splashImageRect.Height;
            extendedSplashImage.Width = splashImageRect.Width;

        }

        void PositionRing() {
            splashProgressRing.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (splashProgressRing.Width * 0.5));
            splashProgressRing.SetValue(Canvas.TopProperty, (splashImageRect.Y + splashImageRect.Height + splashImageRect.Height * 0.1));
        }

        void ExtendedSplash_OnResize(Object sender, WindowSizeChangedEventArgs e) {
            // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
            if (splash != null) {
                // Update the coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();
                PositionRing();
            }
        }

        // Include code to be executed when the system has transitioned from the splash screen to the extended splash screen (application's first view).
        void DismissedEventHandler(SplashScreen sender, object e) {
            dismissed = true;

            // Complete app setup operations here...
        }

        void DismissExtendedSplash() {
            // Navigate to mainpage
            rootFrame.Navigate(typeof(MainPage));
            // Place the frame in the current Window
            Window.Current.Content = rootFrame;
        }

        void DismissSplashButton_Click(object sender, RoutedEventArgs e) {
            DismissExtendedSplash();
        }
        
        private DispatcherTimer showWindowTimer;
        private void OnShowWindowTimer(object sender, object e) {
            showWindowTimer.Stop();

            // Activate/show the window, now that the splash image has rendered
            Window.Current.Activate();
        }

        private void extendedSplashImage_ImageOpened(object sender, RoutedEventArgs e) {
            // ImageOpened means the file has been read, but the image hasn't been painted yet.
            // Start a short timer to give the image a chance to render, before showing the window
            // and starting the animation.
            showWindowTimer = new DispatcherTimer();
            showWindowTimer.Interval = TimeSpan.FromMilliseconds(50);
            showWindowTimer.Tick += OnShowWindowTimer;
            showWindowTimer.Start();
        }
    }
}
