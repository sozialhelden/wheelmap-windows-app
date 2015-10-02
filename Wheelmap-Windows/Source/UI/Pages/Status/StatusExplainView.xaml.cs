using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Wheelmap.Model;
using Windows.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using Wheelmap.Source.Utils.Interfaces;
using Windows.UI;

namespace Wheelmap.Source.UI.Pages.Status {
    public sealed partial class StatusExplainView : UserControl, Selectable {

        private bool _Selected = false;
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

        public Brush ForegroundColor {
            get {
                return statusTitle.Foreground;
            }
            set {
                statusTitle.Foreground = value;
            }
        }

        private bool _ShowBorder = false;
        public bool ShowBorder {
            get {
                return _ShowBorder;
            }
            set {
                _ShowBorder = value;
                if (value) {
                    border.Background = new SolidColorBrush(Status.GetColor());
                } else {
                    border.Background = new SolidColorBrush(Colors.Transparent);
                }
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

        private bool _IsSelectable = true;
        public bool IsSelectable {
            get {
                return _IsSelectable;
            }
            set {
                _IsSelectable = value;
                if (value) {
                    toggleStatusBox.Visibility = Visibility.Visible;
                } else {
                    toggleStatusBox.Visibility = Visibility.Collapsed;
                }
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

            // update border background
            ShowBorder = _ShowBorder;
        }

        private void UpdateHints() {
            statusHintsContainer.Children.Clear();
            string[] hints = Status.GetHints(IsWcStatus);
            foreach(string hint in hints) {
                TextBlock b = new TextBlock();
                b.Text = "\u00B7 " + hint;
                b.TextWrapping = TextWrapping.WrapWholeWords;
                b.Foreground = ForegroundColor;
                statusHintsContainer.Children.Add(b);
            }
        }
        
        private void ToggleStatus_Tapped(object sender, TappedRoutedEventArgs e) {
            Selected = !Selected;
        }
    }
}
