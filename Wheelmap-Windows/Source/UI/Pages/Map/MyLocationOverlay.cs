using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Utils;
using Wheelmap_Windows.Utils.Eventbus;
using Wheelmap_Windows.Utils.Eventbus.Events;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Wheelmap_Windows.Source.UI.Pages.Map {
    public class MyLocationOverlay {

        MapControl mMapControl;
        DependencyObject mMarker;

        public MyLocationOverlay(MapControl mapControl) {
            mMapControl = mapControl;
            BusProvider.DefaultInstance.Register(this);
            OnLocationChanged(LocationManager.Instance.LastLocationEvent);
        }

        private UIElement CreateMarker() {
            Canvas marker = new Canvas();
            Ellipse outer = new Ellipse() {
                Width = 25,
                Height = 25,
            };
            outer.Fill = new SolidColorBrush(Color.FromArgb(255,240,240,240));
            outer.Margin = new Thickness(-12.5, -12.5, 0, 0);

            Ellipse inner = new Ellipse() {
                Width = 20,
                Height = 20,
            };
            inner.Fill = new SolidColorBrush(Colors.Black);
            inner.Margin = new Thickness(-10, -10, 0, 0);

            Ellipse core = new Ellipse() {
                Width = 10,
                Height = 10,
            };
            core.Fill = new SolidColorBrush(Colors.White);
            core.Margin = new Thickness(-5, -5, 0, 0);
            marker.Children.Add(outer);
            marker.Children.Add(inner);
            marker.Children.Add(core);

            return marker;
        }
        
        [Subscribe]
        public void OnLocationChanged(LocationChangedEvent e) {

            if (e == null) {
                return;
            }

            if (mMarker != null) {
                mMapControl.Children.Remove(mMarker);
            }

            mMarker = CreateMarker();
            mMapControl.Children.Add(mMarker);
            MapControl.SetLocation(mMarker, e.Args.Position.Coordinate.Point);
            MapControl.SetNormalizedAnchorPoint(mMarker, new Point(0.5, 0.5));
        }

        public void Unregister() {
            BusProvider.DefaultInstance.Unregister(this);
        }

    }
}
