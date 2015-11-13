using System;
using System.Collections.Generic;
using Wheelmap.Extensions;
using Wheelmap.UI.Pages.Base;
using Wheelmap.Utils;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Wheelmap.Source.UI.Pages.Intro {

    public sealed partial class IntroPage : BasePage {

        /// <summary>
        /// Action to close the dialog in which the page is placed
        /// </summary>
        Action DismissDialog;

        ToggleGroup<Ellipse> mFlipViewIndicatorToggleGroup = new ToggleGroup<Ellipse>();

        public IntroPage() {
            this.InitializeComponent();
            readyButton.Text = "READY".t();
            initPageIndicator();

        }

        private void initPageIndicator() {
            mFlipViewIndicatorToggleGroup.StateChanged += (item, selected) => {
                if (item == null) {
                    return;
                }
                if (selected) {
                    item.Fill = new SolidColorBrush(color: Color.FromArgb(0xff, 0x99, 0xc1, 0x5c));
                } else {
                    item.Fill = new SolidColorBrush(color: Colors.Black);
                }
            };
            foreach (var child in mPageIndicatorLayout.Children) {
                if (child is Ellipse) {
                    mFlipViewIndicatorToggleGroup.Items.Add((Ellipse)child);
                }
            }
            mFlipViewIndicatorToggleGroup.SelectedItem = mFlipViewIndicatorToggleGroup.Items[0];
        }
        
        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            int selectedIndex = mFlipView.SelectedIndex;
            FillFlipViewPageWithContent(selectedIndex);
            if (mFlipViewIndicatorToggleGroup.Items.Count > selectedIndex && selectedIndex >= 0) {
                mFlipViewIndicatorToggleGroup.SelectedItem = mFlipViewIndicatorToggleGroup.Items[selectedIndex];
            }
        }

        /// <summary>
        /// dynamically fill the frames with the content pages
        /// </summary>
        /// <param name="index">
        /// the position of the flipview. 
        /// 0 <= index <  mFlipView.Items.Count
        /// </param>
        private void FillFlipViewPageWithContent(int index) {
            switch (index) {
                case 0:
                    introFrame0.Navigate(typeof(IntroContentPage1), new IntroArguments {
                        Title = "INTRODUCTION_01_TITLE".t(),
                        Text = "INTRODUCTION_01_TEXT".t(),
                        ImageUri = "ms-appx:///Assets/Images/Intro/introduction_01.png"
                    });
                    break;
                case 1:
                    introFrame1.Navigate(typeof(IntroContentPage1), new IntroArguments {
                        Title = "INTRODUCTION_02_TITLE".t(),
                        Text = "INTRODUCTION_02_TEXT".t(),
                        ImageUri = "ms-appx:///Assets/Images/Intro/introduction_02.png"
                    });
                    break;
                case 2:
                    introFrame2.Navigate(typeof(IntroContentPage1), new IntroArguments {
                        Title = "INTRODUCTION_03_TITLE".t(),
                        Text = "INTRODUCTION_03_TEXT".t(),
                        ImageUri = "ms-appx:///Assets/Images/Intro/introduction_03.png"
                    });
                    break;
                case 3:
                    introFrame3.Navigate(typeof(IntroContentPage1), new IntroArguments {
                        Title = "INTRODUCTION_04_TITLE".t(),
                        Text = "INTRODUCTION_04_TEXT".t(),
                        ImageUri = "ms-appx:///Assets/Images/Intro/introduction_04.png"
                    });
                    break;
                case 4:
                    introFrame4.Navigate(typeof(IntroContentPage2), new IntroArguments {
                        Title = "INTRODUCTION_05_TITLE".t(),
                        Text = "INTRODUCTION_05_TEXT".t(),
                    });
                    break;
            }
        }
        
        private void Ready_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            DismissDialog?.Invoke();
        }

        public static void ShowInDialog() {

            var dialog = new ContentDialog() {
                Template = Application.Current.Resources["ButtonLessContentDialogControlTemplate"] as ControlTemplate,
                RequestedTheme = ElementTheme.Light
            };

            var frame = new Frame();
            frame.Navigate(typeof(IntroPage), null);
            var page = frame.Content as IntroPage;
            page.DismissDialog = () => {
                dialog.Hide();
            };
            dialog.Content = frame;
            dialog.ShowAsync().Forget();
        }
        
    }
}
