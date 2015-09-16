using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap.Extensions;
using Wheelmap.UI.Pages.Base;
using Wheelmap.Utils;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap.Source.UI.Pages.Status {
    
    public partial class StatusPage : BasePage {

        public override string Title {
            get {
                return IsWcStatus ? "TITLE_WC".t().ToUpper() : "TITLE_STATUS".t().ToUpper();
            }
        }

        public string Hint {
            get {
                return IsWcStatus ? "STATUS_TITLE_WC_HINT".t(R.File.STATUS) : "STATUS_TITLE_HINT".t(R.File.STATUS);
            }
        }

        public bool IsWcStatus = true;

        public StatusPage() {
            this.InitializeComponent();
            WindowSizeStates.CurrentStateChanged += (sender, e) => OnStateChanged(WindowSizeStates.CurrentState);
            OnStateChanged(WindowSizeStates.CurrentState);
            OnNewParams(true);
        }

        public override void OnNewParams(object args) {
            base.OnNewParams(args);
            if (args == null) {
                return;
            }
            IsWcStatus = (bool) args;
            statusYesView.IsWcStatus = IsWcStatus;
            statusLimitedView.IsWcStatus = IsWcStatus;
            statusNoView.IsWcStatus = IsWcStatus;
            statusUnknownView.IsWcStatus = IsWcStatus;

            if (IsWcStatus) {
                statusLimitedView.Visibility = Visibility.Collapsed;
            } else {
                statusLimitedView.Visibility = Visibility.Visible;
            }

            titleTextBlock.Text = Title;
            hintTextBlock.Text = Hint;

            RestoreSelectionStates();
        }

        private void RestoreSelectionStates() {
            var filter = DataHolder.Instance.Filter;
            var set = IsWcStatus ? filter.FilterdWcStati : filter.FilterdStati;

            statusYesView.Selected = !set.Contains(Model.Status.YES);
            statusLimitedView.Selected = !set.Contains(Model.Status.LIMITED);
            statusNoView.Selected = !set.Contains(Model.Status.NO);
            statusUnknownView.Selected = !set.Contains(Model.Status.UNKNOWN);
            
        }

        private void OnStateChanged(VisualState state) {
            var showHints = state != STATE_SMALL;
            statusYesView.ShowStatusHints = showHints;
            statusLimitedView.ShowStatusHints = showHints;
            statusNoView.ShowStatusHints = showHints;
            statusUnknownView.ShowStatusHints = showHints;
        }

        private void StatusExplainView_SelectedStateChanged(StatusExplainView sender, bool selected) {
            bool changed = false;
            if (IsWcStatus) {
                if (selected) {
                    if (DataHolder.Instance.Filter.FilterdWcStati.Contains(sender.Status)) {
                        DataHolder.Instance.Filter.FilterdWcStati.Remove(sender.Status);
                        changed = true;
                    }
                } else {
                    if (!DataHolder.Instance.Filter.FilterdWcStati.Contains(sender.Status)) {
                        DataHolder.Instance.Filter.FilterdWcStati.Add(sender.Status);
                        changed = true;
                    }
                }
            } else {
                if (selected) {
                    if (DataHolder.Instance.Filter.FilterdStati.Contains(sender.Status)) {
                        DataHolder.Instance.Filter.FilterdStati.Remove(sender.Status);
                        changed = true;
                    }
                } else {
                    if (!DataHolder.Instance.Filter.FilterdStati.Contains(sender.Status)) {
                        DataHolder.Instance.Filter.FilterdStati.Add(sender.Status);
                        changed = true;
                    }
                }
            }
            if (changed) {
                DataHolder.Instance.RefreshFilter();
            }
        }
    }
}
