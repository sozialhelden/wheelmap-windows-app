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
using Wheelmap_Windows.Model;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Wheelmap_Windows.Source.UI.Pages.Status {
    public sealed partial class StatusExplainView : UserControl {

        private bool _Selected = true;
        public bool Selected {
            get {
                return _Selected;
            }
            set {
                _Selected = value;
                UpdateSelectedState();
            }
        }


        public Model.Status _Status;
        public Model.Status Status {
            get {
                return _Status;
            }
            set {
                _Status = value;
                UpdateStatus();
            }
        }

        public bool _IsWcStatus;
        public bool IsWcStatus {
            get {
                return _IsWcStatus;
            }
            set {
                _IsWcStatus = value;
                UpdateStatus();
            }
        }


        public bool _ShowStatusHints;
        public bool ShowStatusHints {
            get {
                return _ShowStatusHints;
            }
            set {
                _ShowStatusHints = value;
                statusHintsContainer.Visibility = _ShowStatusHints 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;
            }
        }


        public StatusExplainView() {
            this.InitializeComponent();
            UpdateSelectedState();
        }

        private void UpdateStatus() {
            statusImage.Source = new BitmapImage(new
                          Uri(Status.GetImage(), UriKind.RelativeOrAbsolute));
            if (IsWcStatus) {
                statusTitle.Text = Status.GetLocalizedToiletMessage().ToUpper();
            } else {
                statusTitle.Text = Status.GetLocalizedMessage().ToUpper();
            }
        }

        private void UpdateSelectedState() {
            string toggleStatusImageUri;
            if (Selected) {
                toggleStatusImageUri = "ms-appx:///Assets/Images/ic_choosen_active.png";
            } else {
                toggleStatusImageUri = "ms-appx:///Assets/Images/ic_choosen.png";
            }

            toggleStatusImage.Source = new BitmapImage(new
                      Uri(toggleStatusImageUri, UriKind.RelativeOrAbsolute));
        }
        
        private void ToggleStatus_Tapped(object sender, TappedRoutedEventArgs e) {
            Selected = !Selected;
        }
    }
}
