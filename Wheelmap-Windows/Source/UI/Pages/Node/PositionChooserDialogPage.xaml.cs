using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap.Extensions;
using Wheelmap.UI.Pages.Base;
using Wheelmap.Utils;
using Wheelmap.Utils.Eventbus;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap.Source.UI.Pages.Node {
    
    public sealed partial class PositionChooserDialogPage : BasePage {

        public PositionChooserDialogPage() {
            this.InitializeComponent();

            mapControl.CenterChanged += MapControl_CenterChanged;
            mapControl.ZoomLevel = MapPage.MAP_ZOOM_DEFAULT;
            mapControl.Center = MapPage.DEFAULT_POSITION;

            if (LocationManager.Instance.LastLocationEvent != null) {
                mapControl.ZoomLevel = 16;
                mapControl.Center = LocationManager.Instance.LastLocationEvent.Args.Position.Coordinate.Point;
            }
            
        }

        public override void OnNewParams(object args) {
            base.OnNewParams(args);
            if (args == null) {
                return;
            }
           
            var arg = args as PositionChooserDialogPageArgs;

            mapControl.Center = new Geopoint(new BasicGeoposition {Latitude = arg.lat, Longitude = arg.lon });
            mapControl.ZoomLevel = 17;

        }

        private void MapControl_CenterChanged(Windows.UI.Xaml.Controls.Maps.MapControl sender, object args) {
            currentPositionTextBlock.Text = String.Format("POSITION".t(R.File.NODE), mapControl.Center.Position.Latitude, mapControl.Center.Position.Longitude);
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e) {
            mapControl.ZoomLevel += 1;
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e) {
            mapControl.ZoomLevel -= 1;
        }

        public void OnMyPosition_Click(object sender, RoutedEventArgs e) {
            var point = LocationManager.Instance?.LastLocationEvent?.Args?.Position?.Coordinate?.Point;
            if (point != null) {
                mapControl.Center = point;
                mapControl.ZoomLevel = 17;
            }
        }

        private void OnCompass_Click(object sender, RoutedEventArgs e) {
            mapControl.TryRotateToAsync(0).AsTask().Forget();
        }

        public static void ShowInDialog(PositionChooserDialogPageArgs args, Action<BasicGeoposition> finishedAction) {
            var dialog = new ContentDialog() {
                Template = Application.Current.Resources["MyContentDialogControlTemplate"] as ControlTemplate,
                RequestedTheme = ElementTheme.Light
            };
            dialog.Padding = new Thickness(-10, -10, -10, -10);
            dialog.BorderThickness = new Thickness(0, 0, 0, 0);

            var frame = new Frame();
            frame.Navigate(typeof(PositionChooserDialogPage), args);
            var page = frame.Content as PositionChooserDialogPage;
            dialog.Content = frame;

            dialog.PrimaryButtonText = "SAVE".t(R.File.NODE);
            dialog.PrimaryButtonClick += (d, _) => {
                finishedAction?.Invoke(page.mapControl.Center.Position);
            };
            dialog.ShowAsync().Forget();
        }
    }

    public class PositionChooserDialogPageArgs {
        public double lat;
        public double lon;
    }
}
