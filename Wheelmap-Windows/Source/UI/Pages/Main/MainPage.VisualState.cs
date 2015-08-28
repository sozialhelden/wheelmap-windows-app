using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.UI.Pages.Base;
using Wheelmap_Windows.Utils;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Wheelmap_Windows.Source.UI {

    /**
     * part of MainPage to handle all viszal state changes
     */
    public sealed partial class MainPage : BasePage {

        const string TAG = "MainPage";
        
        private VisualState CurrentSizeState {
            get {
                return WindowSizeStates.CurrentState;
            }
        }

        private void InitVisualState() {
            WindowSizeStates.CurrentStateChanged += (sender, e) => OnStateChanged(e.NewState);
            Loaded += (sender, e) => OnStateChanged(WindowSizeStates.CurrentState);
        }
        
        /**
         * Do some extra action on VisualStateChanged.
         */
        private void OnStateChanged(VisualState state) {
            Log.d(TAG, "New State Changed: " + state.Name);

            if (state == STATE_SMALL) {
                phoneUIControlsTop.Visibility = Visibility.Visible;
                phoneUIControlsBottom.Visibility = Visibility.Visible;
                if (detailContainerFrame.Content != null || SecondPage.Content != null) {
                    bottomBar.Visibility = Visibility.Collapsed;
                } else {
                    bottomBar.Visibility = Visibility.Visible;
                }
            } else if (state == STATE_MEDIUM) {
                phoneUIControlsTop.Visibility = Visibility.Collapsed;
                phoneUIControlsBottom.Visibility = Visibility.Collapsed;
                bottomBar.Visibility = Visibility.Collapsed;
            } else {
                phoneUIControlsTop.Visibility = Visibility.Collapsed;
                phoneUIControlsBottom.Visibility = Visibility.Collapsed;
                bottomBar.Visibility = Visibility.Collapsed;
            }

            this.RefreshCanGoBack();
        }
        
    }
}
