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

namespace Wheelmap_Windows.Source.UI {
    
    public sealed partial class MainPage : Page {

        public MainPage() {

            this.InitializeComponent();
            mainFrame.Navigate(typeof(MapPage));
            //var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            
            BusProvider.DefaultInstance.Register(this);
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e) {
            toggleMenu();
        }

        private void ShowMenu(bool show) {
            mSplitView.IsPaneOpen = show;
        }

        private void toggleMenu() {
            // toggle menu
            ShowMenu(!mSplitView.IsPaneOpen);
        }
        
        private void Button_Click(object sender, RoutedEventArgs e) {
            Debug.WriteLine("Button Clicked");
            menuContainerFrame.Navigate(typeof(MenuPage));
            menuContainerFrame.Visibility = Visibility.Visible;
        }

        private void Menu_Click(object sender, RoutedEventArgs e) {
            Debug.WriteLine("Menu Clicked");

            if (menuContainerFrame.Visibility == Visibility.Collapsed) {
                menuContainerFrame.Visibility = Visibility.Visible;
            } else {
                menuContainerFrame.Visibility = Visibility.Collapsed;
            }

        }

        private void ShowListTapped(object sender, TappedRoutedEventArgs e) {

            ShowMenu(false);

            Debug.WriteLine("ShowListTapped Clicked");
            
            if (menuContainerFrame.Content is NodeListPage) {
                // remove content
                (menuContainerFrame.Content as NodeListPage).Unregister();
                menuContainerFrame.Content = null;
                return;
            }

            menuContainerFrame.Navigate(typeof(NodeListPage));

        }

        [Subscribe]
        public void OnSelectedNodeChanged(SelectedNodeChangedEvent e) {
            detailContainerFrame.Navigate(typeof(NodeDetailPage), e.node);
        }

    }
}
