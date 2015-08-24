using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.UI.Xaml.Controls.Maps;
using Wheelmap_Windows.Source.UI.Pages;
using Wheelmap_Windows.Source.UI.Pages.List;
using Wheelmap_Windows.Api.Calls;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.Utils.Eventbus;
using Wheelmap_Windows.Utils.Eventbus.Events;
using Wheelmap_Windows.Source.UI.Pages.Node;
using Windows.UI;
using Windows.UI.Core;
using Wheelmap_Windows.Utils;
using Wheelmap_Windows.Extensions;
using Wheelmap_Windows.Source.UI.Pages.Categories;
using Windows.Devices.Geolocation;

namespace Wheelmap_Windows.Source.UI {

    public sealed partial class MainPage : Page {

        ToggleGroup<Panel> mToggleGroup;

        public MainPage() {

            this.InitializeComponent();
            InitToggleGroup();

            mainFrame.Navigate(typeof(MapPage));
            //var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            BusProvider.DefaultInstance.Register(this);

            SystemNavigationManager.GetForCurrentView().BackRequested += (s, e) => {
                if (detailContainerFrame.Content != null) {
                    detailContainerFrame.Content = null;
                    SetBackButtonStatus();
                    return;
                }
                if (menuContainerFrame.Content != null) {
                    menuContainerFrame.Content = null;
                    SetBackButtonStatus();
                    return;
                }
                SetBackButtonStatus();
            };

            SetBackButtonStatus();

            new CategoryRequest().Query().ContinueWith((categories) => {
                DataHolder.Instance.Categories.Clear();
                foreach (Category c in categories.Result) {
                    DataHolder.Instance.Categories.Add(c.identifier, c);
                }
            });
            
        }

        private void InitToggleGroup() {
            mToggleGroup = new ToggleGroup<Panel>();
            foreach (Panel p in menuTopPanel.Children) {
                mToggleGroup.Items.Add(p);
            }
            foreach (Panel p in menuBottomPanel.Children) {
                mToggleGroup.Items.Add(p);
            }
            mToggleGroup.Delegate = (item, selected) => {
                if (item == null) {
                    return;
                }
                if (selected) {
                    item.Background = new SolidColorBrush(Color.FromArgb(0xff, 0x99, 0xc1, 0x5c));
                } else {
                    item.Background = new SolidColorBrush(Color.FromArgb(0x00, 0x00, 0x00, 0x00));
                }
            };
        }

        private void SetBackButtonStatus() {
            if (menuContainerFrame.Content != null || detailContainerFrame.Content != null) {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            } else {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                mToggleGroup.SelectedItem = null;
            }
        }

        private void ShowMenu(bool show) {
            mSplitView.IsPaneOpen = show;
        }

        private void toggleMenu() {
            // toggle menu
            ShowMenu(!mSplitView.IsPaneOpen);
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e) {
            toggleMenu();
        }

        private void ShowListTapped(object sender, TappedRoutedEventArgs e) {
            ShowOnMenuContainerFrame(sender, typeof(NodeListPage));
        }

        private void ShowHelpTapped(object sender, TappedRoutedEventArgs e) {
            ShowMenu(false);
            Debug.WriteLine("ShowHelpTapped Clicked");
        }

        private void ShowWCTapped(object sender, TappedRoutedEventArgs e) {
            ShowMenu(false);
            Debug.WriteLine("ShowWCTapped Clicked");
        }

        private void ShowStatusTapped(object sender, TappedRoutedEventArgs e) {
            ShowMenu(false);
            Debug.WriteLine("ShowStatusTapped Clicked");
        }

        private void ShowCategoryTapped(object sender, TappedRoutedEventArgs e) {
            Debug.WriteLine("ShowCategoryTapped Clicked");
            ShowOnMenuContainerFrame(sender, typeof(CategoriesListPage));
        }

        private void ShowProfileTapped(object sender, TappedRoutedEventArgs e) {
            ShowMenu(false);
            Debug.WriteLine("ShowProfileTapped Clicked");
        }

        private void ShowSettingsTapped(object sender, TappedRoutedEventArgs e) {
            ShowMenu(false);
            Debug.WriteLine("ShowSettingsTapped Clicked");
        }

        private void ShowOnMenuContainerFrame(object sender, Type pageType) {
            ShowMenu(false);

            if (menuContainerFrame.Content?.GetType() == pageType) {
                // remove content
                if (menuContainerFrame.Content is Page) {
                    (menuContainerFrame.Content as Page).Unregister();
                }
                menuContainerFrame.Content = null;
                SetBackButtonStatus();
                return;
            }

            menuContainerFrame.Navigate(pageType);
            SetBackButtonStatus();

            if (sender is Panel) {
                if (!mToggleGroup.Items.Contains(sender)) {
                    mToggleGroup.Items.Add(sender as Panel);
                }
                mToggleGroup.SelectedItem = (sender as Panel);
            }

        }

        [Subscribe]
        public void OnSelectedNodeChanged(SelectedNodeChangedEvent e) {
            detailContainerFrame.Navigate(typeof(NodeDetailPage), e.node);
            SetBackButtonStatus();
        }
        
    }
}
