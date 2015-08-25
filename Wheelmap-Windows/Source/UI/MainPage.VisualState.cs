using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Wheelmap_Windows.Source.UI {

    public sealed partial class MainPage : Page {

        const string TAG = "MainPage";

        private void InitVisualState() {
            WindowSizeStates.CurrentStateChanged += WindowSizeStates_CurrentStateChanged;
        }

        private void WindowSizeStates_CurrentStateChanged(object sender, VisualStateChangedEventArgs e) {
            Log.d(TAG, "New State Changed: "+ e.NewState.Name);

            if (e.NewState == STATE_SMALL) {
                Grid.SetColumn(menuContainerFrame, 2);
                Grid.SetColumn(detailContainerFrame, 2);
            } else if (e.NewState == STATE_MEDIUM){
                Grid.SetColumn(menuContainerFrame, 0);
                Grid.SetColumn(detailContainerFrame, 0);
            } else {
                Grid.SetColumn(menuContainerFrame, 0);
                Grid.SetColumn(detailContainerFrame, 1);
            }
        }
        
    }
}
