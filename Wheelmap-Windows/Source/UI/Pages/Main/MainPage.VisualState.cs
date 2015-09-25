using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Source.UI.Pages.Categories;
using Wheelmap.Source.UI.Pages.List;
using Wheelmap.Source.UI.Pages.Status;
using Wheelmap.UI.Pages.Base;
using Wheelmap.Utils;
using Wheelmap_Windows.Source.UI.Pages.Settings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Wheelmap.Source.UI {

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
                OnSmallState();
            } else if (state == STATE_MEDIUM) {
                OnMediumState();
            } else {
                OnBigState();
            }

            this.RefreshCanGoBack();
        }

        private void OnSmallState() {
            phoneUIControlsTop.Visibility = Visibility.Visible;
            phoneUIControlsBottom.Visibility = Visibility.Visible;
            if (detailContainerFrame.Content != null || SecondPage.Content != null) {
                bottomBar.Visibility = Visibility.Collapsed;
            } else {
                bottomBar.Visibility = Visibility.Visible;
            }

            if (menuContainerFrame.Content is StatusPage
                || menuContainerFrame.Content is CategoriesListPage) {
                var param = (menuContainerFrame.Content as BasePage).Parameter;
                var type = menuContainerFrame.Content.GetType();
                menuContainerFrame.Content = null;
                phoneUIBottomSlideUp.Navigate(type, param);
                backgroundOverlay.Visibility = Visibility.Visible;
            }
            
            detailContainerFrame.ContentTransitions = new TransitionCollection {
                new EdgeUIThemeTransition { Edge = EdgeTransitionLocation.Bottom }
            };
            
            menuContainerFrame.ContentTransitions = new TransitionCollection { };
            
            var menuBasePage = menuContainerFrame.Content as BasePage;
            var detailBasePage = detailContainerFrame.Content as BasePage;
            
            switch (menuBasePage?.GetShowOnSmall()) {
                case PageShowOn.DETAIL:
                    menuContainerFrame.Content = null;
                    mToggleGroup.SelectedItem = null;
                    ShowOnDetailFrame(menuBasePage.GetType(), menuBasePage.Parameter);
                    break;
                case PageShowOn.NONE:
                    menuContainerFrame.Content = null;
                    mToggleGroup.SelectedItem = null;
                    break;
            }

            switch (detailBasePage?.GetShowOnSmall()) {
                case PageShowOn.MENU:
                    if (detailContainerFrame.Content == detailBasePage) {
                        detailContainerFrame.Content = null;
                    }
                    ShowOnMenuContainerFrame(null, detailBasePage.GetType(), param: detailBasePage.Parameter);
                    break;
                case PageShowOn.NONE:
                    if (detailContainerFrame.Content == detailBasePage) {
                        detailContainerFrame.Content = null;
                    }
                    break;
            }
            
        }

        private void OnMediumState() {
            OnBigState();
        }

        private void OnBigState() {
            phoneUIControlsTop.Visibility = Visibility.Collapsed;
            phoneUIControlsBottom.Visibility = Visibility.Collapsed;
            bottomBar.Visibility = Visibility.Collapsed;
            searchContainer.Visibility = Visibility.Collapsed;

            if (phoneUIBottomSlideUp.Content is StatusPage
                || phoneUIBottomSlideUp.Content is CategoriesListPage) {
                var param = (phoneUIBottomSlideUp.Content as BasePage).Parameter;
                var type = phoneUIBottomSlideUp.Content.GetType();
                phoneUIBottomSlideUp.Content = null;
                backgroundOverlay.Visibility = Visibility.Collapsed;
                ShowOnMenuContainerFrame(null, type, param);
            }

            menuContainerFrame.ContentTransitions = new TransitionCollection {
                new EdgeUIThemeTransition { Edge = EdgeTransitionLocation.Left }
            };
            
            detailContainerFrame.ContentTransitions = new TransitionCollection {};
            
            var menuBasePage = menuContainerFrame.Content as BasePage;
            var detailBasePage = detailContainerFrame.Content as BasePage;

            switch (menuBasePage?.GetShowOnBig()) {
                case PageShowOn.DETAIL:
                    menuContainerFrame.Content = null;
                    ShowOnDetailFrame(menuBasePage.GetType(), menuBasePage.Parameter);
                    break;
                case PageShowOn.NONE:
                    menuContainerFrame.Content = null;
                    mToggleGroup.SelectedItem = null;
                    break;
            }

            switch (detailBasePage?.GetShowOnBig()) {
                case PageShowOn.MENU:
                    if (detailContainerFrame.Content == detailBasePage) {
                        detailContainerFrame.Content = null;
                    }
                    ShowOnMenuContainerFrame(null, detailBasePage.GetType(), param: detailBasePage.Parameter);
                    break;
                case PageShowOn.NONE:
                    if (detailContainerFrame.Content == detailBasePage) {
                        detailContainerFrame.Content = null;
                    }
                    break;
            }
            
        }

    }
}
