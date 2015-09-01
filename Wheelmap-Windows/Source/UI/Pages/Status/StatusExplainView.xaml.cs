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
using System.ComponentModel;

namespace Wheelmap_Windows.Source.UI.Pages.Status {
    public sealed partial class StatusExplainView : UserControl, INotifyPropertyChanged {

        private bool _Selected = true;
        public bool Selected {
            get {
                return _Selected;
            }
            set {
                _Selected = value;
                toggleStatusBox.IsChecked = value;
                SelectedStateChanged?.Invoke(this, _Selected);
                NotifyPropertyChanged(nameof(Selected));
            }
        }

        public event TypedEventHandler<StatusExplainView, bool> SelectedStateChanged;
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

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

        public bool _IsWcStatus = false;
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
        }

        private void UpdateStatus() {
            statusImage.Source = new BitmapImage(new
                          Uri(Status.GetImage(), UriKind.RelativeOrAbsolute));
            if (IsWcStatus) {
                statusTitle.Text = Status.GetLocalizedToiletMessage().ToUpper();
            } else {
                statusTitle.Text = Status.GetLocalizedMessage().ToUpper();
            }

            UpdateHints();
        }

        private void UpdateHints() {
            statusHintsContainer.Children.Clear();
            string[] hints = Status.GetHints(IsWcStatus);
            foreach(string hint in hints) {
                TextBlock b = new TextBlock();
                b.Text = "\u00B7 " + hint;
                b.TextWrapping = TextWrapping.WrapWholeWords;
                statusHintsContainer.Children.Add(b);
            }
        }
        
        private void ToggleStatus_Tapped(object sender, TappedRoutedEventArgs e) {
            Selected = !Selected;
        }
    }
}
