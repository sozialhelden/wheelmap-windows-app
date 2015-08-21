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

            ShowMenu(false);

            Debug.WriteLine("ShowListTapped Clicked");
            
            if (menuContainerFrame.Content is NodeListPage) {
                // remove content
                (menuContainerFrame.Content as NodeListPage).Unregister();
                menuContainerFrame.Content = null;
                SetBackButtonStatus();
                return;
            }
            
            menuContainerFrame.Navigate(typeof(NodeListPage));
            SetBackButtonStatus();
            mToggleGroup.SelectedItem = (sender as Panel);

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
            ShowMenu(false);
            Debug.WriteLine("ShowCategoryTapped Clicked");
        }

        private void ShowProfileTapped(object sender, TappedRoutedEventArgs e) {
            ShowMenu(false);
            Debug.WriteLine("ShowProfileTapped Clicked");
        }

        private void ShowSettingsTapped(object sender, TappedRoutedEventArgs e) {
            ShowMenu(false);
            Debug.WriteLine("ShowSettingsTapped Clicked");
        }
        
        [Subscribe]
        public void OnSelectedNodeChanged(SelectedNodeChangedEvent e) {
            detailContainerFrame.Navigate(typeof(NodeDetailPage), e.node);
            SetBackButtonStatus();
        }

    }
}
