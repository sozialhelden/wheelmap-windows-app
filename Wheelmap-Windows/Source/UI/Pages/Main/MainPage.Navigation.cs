using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Extensions;
using Wheelmap_Windows.Source.UI.Pages.Categories;
using Wheelmap_Windows.Source.UI.Pages.Node;
using Wheelmap_Windows.Source.UI.Pages.Status;
using Wheelmap_Windows.UI.Pages.Base;
using Wheelmap_Windows.Utils;
using Wheelmap_Windows.Utils.Eventbus;
using Wheelmap_Windows.Utils.Eventbus.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Wheelmap_Windows.Source.UI {

    /**
     * part of MainPage to handle all navigations
     */
    public sealed partial class MainPage : BasePage {

        public override bool CanGoBack() {
            if (CurrentSizeState == STATE_SMALL) {
                if (detailContainerFrame.Content != null || SecondPage.Content != null) {
                    bottomBar.Visibility = Visibility.Collapsed;
                } else {
                    bottomBar.Visibility = Visibility.Visible;
                }
                return SecondPage.Content != null 
                    || detailContainerFrame.Content != null
                    || phoneUIBottomSlideUp.Content != null 
                    || searchContainer.Visibility == Visibility.Visible;
            }
            return SecondPage.Content != null
                || menuContainerFrame.Content != null
                || detailContainerFrame.Content != null
                ;
        }

        public override void GoBack() {
            
            if (SecondPage.Content != null) {
                if (SecondPage.CanGoBack) {
                    SecondPage.GoBack();
                } else {
                    SecondPage.Content = null;
                }
                UpdateTitle();
                return;
            }
            
            if (detailContainerFrame.Content != null) {
                if (detailContainerFrame.CanGoBack
                    // the user should always be able to go back from the NodeDetailPage
                    && !(detailContainerFrame.Content is NodeDetailPage)) {
                    detailContainerFrame.GoBack();
                } else {
                    detailContainerFrame.ForwardStack.Clear();
                    detailContainerFrame.BackStack.Clear();
                    detailContainerFrame.Content = null;
                }
                UpdateTitle();
                return;
            }
            
            if (WindowSizeStates.CurrentState == STATE_SMALL && searchContainer.Visibility == Visibility.Visible) {
                searchContainer.Visibility = Visibility.Collapsed;
                DataHolder.Instance.QueryString = null;
                return;
            }

            if (phoneUIBottomSlideUp.Content != null) {
                phoneUIBottomSlideUp.Content = null;
                backgroundOverlay.Visibility = Visibility.Collapsed;
                return;
            }

            if (menuContainerFrame.Content != null) {
                menuContainerFrame.Content = null;
                mToggleGroup.SelectedItem = null;
                UpdateTitle();
                return;
            }
        }

        public void NavigateSecondPage(Type page, object args = null) {
            //SecondPage.Navigate(page, args);
            (App.Current as App).Navigate(page, args);
            this.RefreshCanGoBack();
        }

        [Subscribe]
        public void OnSelectedNodeChanged(SelectedNodeChangedEvent e) {
            if (detailContainerFrame.Content is NodeDetailPage) {
                (detailContainerFrame.Content as NodeDetailPage).SetNode(e.node);
            } else {
                ShowOnDetailFrame(typeof(NodeDetailPage), e.node);
            }

            this.RefreshCanGoBack();

        }

        /**
         * returns true if page will be shown
         */
        private bool ShowOnMenuContainerFrame(object sender, Type pageType, object param = null) {
            ShowMenu(false);

            bool ret;
            if (CurrentSizeState == STATE_SMALL) {
                ret = _ShowOnMenuContainerFrameSmall(sender, pageType, param);
            } else {
                ret = _ShowOnMenuContainerFrameNormal(sender, pageType, param);
            }

            this.RefreshCanGoBack();
            UpdateTitle();
            return ret;
        }

        private bool _ShowOnMenuContainerFrameNormal(object sender, Type pageType, object param = null) {
            if (menuContainerFrame.Content?.GetType() == pageType) {

                var page = (menuContainerFrame.Content as BasePage);
                if (page.Parameter?.Equals(param) ?? param == page.Parameter) {
                    if (CurrentSizeState == STATE_SMALL) {
                        // on small screens the menuContainerFrame works like an tabbar
                        return true;
                    }
                    // remove content
                    if (menuContainerFrame.Content is Page) {
                        (menuContainerFrame.Content as Page).Unregister();
                    }
                    menuContainerFrame.Content = null;
                    mToggleGroup.SelectedItem = null;
                    return false;
                } else {
                    if (CurrentSizeState != STATE_SMALL) {
                        menuContainerFrame.Content = null;
                    }
                }
            }

            menuContainerFrame.Navigate(pageType, param);

            if (Grid.GetColumn(menuContainerFrame) == Grid.GetColumn(detailContainerFrame)) {
                detailContainerFrame.Content = null;
            }

            if (sender is Panel) {
                if (!mToggleGroup.Items.Contains(sender)) {
                    mToggleGroup.Items.Add(sender as Panel);
                }
                mToggleGroup.SelectedItem = (sender as Panel);
            }
            phoneUIBottomSlideUp.Content = null;
            backgroundOverlay.Visibility = Visibility.Collapsed;

            return true;

        }

        private bool _ShowOnMenuContainerFrameSmall(object sender, Type pageType, object param = null) {
            if (!(pageType == typeof(CategoriesListPage)
                 || pageType == typeof(StatusPage))) {
                return _ShowOnMenuContainerFrameNormal(sender, pageType, param);
            }

            if (phoneUIBottomSlideUp.Content?.GetType() == pageType) {
                var page = (phoneUIBottomSlideUp.Content as BasePage);
                if (page.Parameter?.Equals(param) ?? page.Parameter == param) {
                    phoneUIBottomSlideUp.Content = null;
                    backgroundOverlay.Visibility = Visibility.Collapsed;
                    return false;
                } else {
                    phoneUIBottomSlideUp.Content = null;
                    backgroundOverlay.Visibility = Visibility.Collapsed;
                }
            }

            phoneUIBottomSlideUp.Navigate(pageType, param);
            backgroundOverlay.Visibility = Visibility.Visible;

            return true;
        }

        public override void ShowOnDetailFrame(Type type, object args = null) {
            var contentEmpty = detailContainerFrame.Content == null;
            detailContainerFrame.Navigate(type, args);

            // workaround to fix back handling
            if (contentEmpty) {
                detailContainerFrame.BackStack.Clear();
            }

            this.RefreshCanGoBack();
        }

        [Subscribe]
        public void OnUserChanged(UserChangedEvent e) {
            GoBack();
        }

    }
}
