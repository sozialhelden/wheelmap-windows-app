﻿using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Wheelmap.Source.UI.Pages;
using Wheelmap.Source.UI.Pages.List;
using Wheelmap.Api.Calls;
using Wheelmap.Utils.Eventbus;
using Wheelmap.Utils.Eventbus.Events;
using Wheelmap.Source.UI.Pages.Node;
using Windows.UI;
using Wheelmap.Utils;
using Wheelmap.Source.UI.Pages.Categories;
using Wheelmap.UI.Pages.Base;
using Wheelmap.Source.UI.Pages.Status;
using Wheelmap.Source.UI.Pages.Profile;
using Wheelmap_Windows.Source.UI.Pages;
using Wheelmap_Windows.Source.UI.Pages.Status;
using Wheelmap.Extensions;
using Wheelmap.Source.UI.Pages.Credits;
using Wheelmap.Source.UI.Pages.Intro;
using Windows.UI.Core;
using Wheelmap.Utils.Preferences;

namespace Wheelmap.Source.UI {

    public sealed partial class MainPage : BasePage {

        public override string Title {
            get {
                return GetTitle();
            }
        }

        ToggleGroup<Panel> mToggleGroup;
        MapPage mMapPage;
        SearchBoxHandler searchHandler;

        public MainPage() {

            this.InitializeComponent();
            rootContainer.FlowDirection = DeviceUtils.GetFlowDirection();

            InitToggleGroup();

            mainFrame.Navigate(typeof(MapPage));
            mMapPage = mainFrame.Content as MapPage;

            BusProvider.DefaultInstance.Register(this);
            
            InitVisualState();
            UpdateTitle();
            
            // important to keep the page state
            this.NavigationCacheMode = NavigationCacheMode.Enabled;

            FilterChanged(new FilterChangedEvent { Filter = DataHolder.Instance.Filter });
            
            searchHandler = new SearchBoxHandler(searchBox);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            BusProvider.DefaultInstance.Register(this);

            if (Prefs.FirstAppStart) {
                // wait some time to awoit blocking the ui
                Dispatcher.PostDelayed(() => {
                    Prefs.FirstAppStart = false;
                    IntroPage.ShowInDialog();
                }, 2000);
            }

        }
        
        private void InitToggleGroup() {
            mToggleGroup = new ToggleGroup<Panel>();
            foreach (Panel p in menuTopPanel.Children) {
                mToggleGroup.Items.Add(p);
            }
            foreach (Panel p in menuBottomPanel.Children) {
                mToggleGroup.Items.Add(p);
            }

            mToggleGroup.StateChanged += (item, selected) => {
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

        private void ShowMapTapped(object sender, TappedRoutedEventArgs e) {
            menuContainerFrame.Content = null;
            detailContainerFrame.Content = null;
            SecondPage.Content = null;
            this.RefreshCanGoBack();
            UpdateTitle();
        }

        private void ShowListTapped(object sender, TappedRoutedEventArgs e) {
            ShowOnMenuContainerFrame(sender, typeof(NodeListPage), toggle: e != null);
        }

        private void ShowHelpTapped(object sender, TappedRoutedEventArgs e) {
            ShowOnMenuContainerFrame(sender, typeof(NodeListPage), new NodeListPageArgs { HelpMode = true}, toggle: e != null);
        }

        private void ShowWCTapped(object sender, TappedRoutedEventArgs e) {
            ShowOnMenuContainerFrame(sender, typeof(StatusPage), param: true, toggle: e != null);
        }

        private void ShowStatusTapped(object sender, TappedRoutedEventArgs e) {
            ShowOnMenuContainerFrame(sender, typeof(StatusPage), param: false, toggle: e != null);
        }

        private void ShowCategoryTapped(object sender, TappedRoutedEventArgs e) {
            Debug.WriteLine("ShowCategoryTapped Clicked");
            ShowOnMenuContainerFrame(sender, typeof(CategoriesListPage), toggle: e != null);
        }

        private void ShowMyLocationTapped(object sender, TappedRoutedEventArgs e) {
            ShowMapTapped(sender, e);
            mMapPage.OnMyPosition_Click(sender, e);
        }

        private void ShowProfileTapped(object sender, TappedRoutedEventArgs e) {
            ShowOnMenuContainerFrame(sender, typeof(ProfilePage), toggle: e != null);
        }

        private void ShowCreditsTapped(object sender, TappedRoutedEventArgs e) {
            ShowOnFrame(sender, typeof(CreditsPage), new BasePageArguments {
                ShowOnSmall = PageShowOn.DETAIL,
                ShowOnBig = PageShowOn.MENU,
            }, e != null);
        }

        private void ShowFAQTapped(object sender, TappedRoutedEventArgs e) {
            ShowOnFrame(sender, typeof(WebViewPage), new WebViewPageArguments {
                Url = "WEBLINK_FAQ".t(R.File.LINKS),
                ShowOnSmall = PageShowOn.DETAIL,
                ShowOnBig = PageShowOn.MENU,
            }, e != null);
        }

        private void ShowNewsTapped(object sender, TappedRoutedEventArgs e) {
            ShowOnFrame(sender, typeof(WebViewPage), new WebViewPageArguments {
                Url = ApiConstants.NEWS_URL,
                ShowOnSmall = PageShowOn.DETAIL,
                ShowOnBig = PageShowOn.MENU,
            }, e != null);
        }

        private void ShowStatusInfoTapped(object sender, TappedRoutedEventArgs e) {
            ShowOnFrame(sender, typeof(PhoneStatusExplainPage), new BasePageArguments {
                ShowOnBig = PageShowOn.NONE,
                ShowOnSmall = PageShowOn.DETAIL
            }, e != null);
        }

        public void SetTitle(string title) {           
            actionbarTitle.Text = title ?? "WHEELMAP";
        }

        public string GetTitle() {
            return actionbarTitle.Text;
        }
        
        public void UpdateTitle() {
            if (detailContainerFrame.Content is BasePage) {
                var page = detailContainerFrame.Content as BasePage;
                SetTitle(page.Title);
                return;
            }
            if (menuContainerFrame.Content is BasePage) {
                var page = menuContainerFrame.Content as BasePage;
                SetTitle(page.Title);
                return;
            }
            SetTitle(mMapPage.Title);
        }
        
        [Subscribe]
        public void FilterChanged(FilterChangedEvent e) {
            if (e.Filter.FilteredCategoryIdentifier.Count() > 0) {
                sideMenuCategoryImage.Visibility = Visibility.Collapsed;
                sideMenuCategoryImageActive.Visibility = Visibility.Visible;

                appBarCategory.Visibility = Visibility.Collapsed;
                appBarCategoryActive.Visibility = Visibility.Visible;
            } else {
                sideMenuCategoryImage.Visibility = Visibility.Visible;
                sideMenuCategoryImageActive.Visibility = Visibility.Collapsed;

                appBarCategory.Visibility = Visibility.Visible;
                appBarCategoryActive.Visibility = Visibility.Collapsed;
            }

            if (e.Filter.FilterdStati.Count() > 0) {
                sideMenuStatusImage.Visibility = Visibility.Collapsed;
                sideMenuStatusImageActive.Visibility = Visibility.Visible;

                appBarStatus.Visibility = Visibility.Collapsed;
                appBarStatusActive.Visibility = Visibility.Visible;
            } else {
                sideMenuStatusImage.Visibility = Visibility.Visible;
                sideMenuStatusImageActive.Visibility = Visibility.Collapsed;
                
                appBarStatus.Visibility = Visibility.Visible;
                appBarStatusActive.Visibility = Visibility.Collapsed;
            }
            
            if (e.Filter.FilterdWcStati.Count() > 0) {
                sideMenuWCStatusImage.Visibility = Visibility.Collapsed;
                sideMenuWCStatusImageActive.Visibility = Visibility.Visible;

                appBarWCStatus.Visibility = Visibility.Collapsed;
                appBarWCStatusActive.Visibility = Visibility.Visible;
            } else {
                sideMenuWCStatusImage.Visibility = Visibility.Visible;
                sideMenuWCStatusImageActive.Visibility = Visibility.Collapsed;

                appBarWCStatus.Visibility = Visibility.Visible;
                appBarWCStatusActive.Visibility = Visibility.Collapsed;
            }
        }

        private void AddNewNode_Tapped(object sender, TappedRoutedEventArgs e) {
            this.ShowOnDetailFrame(typeof(NodeEditPage));
        }

        private void TappedInBackground(object sender, TappedRoutedEventArgs e) {
            GoBack();
        }

        private void SearchIcon_Tapped(object sender, TappedRoutedEventArgs e) {
            searchContainer.Visibility = Visibility.Visible;
            this.RefreshCanGoBack();
        }
    }
}
