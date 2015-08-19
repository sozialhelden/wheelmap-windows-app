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

namespace Wheelmap_Windows.Source.UI {

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {

        public MainPage() {
            this.InitializeComponent();
            mainFrame.Navigate(typeof(MapPage));
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

            Debug.WriteLine("ShowListTapped Clicked");
            menuContainerFrame.Navigate(typeof(MenuPage));
            ShowMenu(false);

        }
    }
}
