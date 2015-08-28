using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Extensions;
using Wheelmap_Windows.Source.UI.Pages.Node;
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
                return SecondPage.Content != null || detailContainerFrame.Content != null;
            }
            return SecondPage.Content != null
                || menuContainerFrame.Content != null
                || detailContainerFrame.Content != null;
        }
        
        public override void GoBack() {
            if (SecondPage.Content != null) {
                SecondPage.Content = null;
                UpdateTitle();
                return;
            }

            if (detailContainerFrame.Content != null) {
                detailContainerFrame.Content = null;
                UpdateTitle();
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
            SecondPage.Navigate(page, args);
            this.RefreshCanGoBack();
        }

        [Subscribe]
        public void OnSelectedNodeChanged(SelectedNodeChangedEvent e) {
            if (detailContainerFrame.Content is NodeDetailPage) {
                (detailContainerFrame.Content as NodeDetailPage).SetNode(e.node);
            } else {
                detailContainerFrame.Navigate(typeof(NodeDetailPage), e.node);
            }
            
            this.RefreshCanGoBack();
            
        }
        
        /**
         * returns true if page will be shown
         */
        private bool ShowOnMenuContainerFrame(object sender, Type pageType) {
            ShowMenu(false);

            if (menuContainerFrame.Content?.GetType() == pageType) {
                // remove content
                if (menuContainerFrame.Content is Page) {
                    (menuContainerFrame.Content as Page).Unregister();
                }
                menuContainerFrame.Content = null;
                mToggleGroup.SelectedItem = null;
                this.RefreshCanGoBack();
                UpdateTitle();
                return false;
            }

            menuContainerFrame.Navigate(pageType);

            if (Grid.GetColumn(menuContainerFrame) == Grid.GetColumn(detailContainerFrame)) {
                detailContainerFrame.Content = null;
            }

            this.RefreshCanGoBack();

            if (sender is Panel) {
                if (!mToggleGroup.Items.Contains(sender)) {
                    mToggleGroup.Items.Add(sender as Panel);
                }
                mToggleGroup.SelectedItem = (sender as Panel);
            }

            UpdateTitle();
            return true;

        }

    }
}
