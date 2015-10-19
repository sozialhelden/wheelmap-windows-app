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

    public sealed partial class ChangeStatusDialogPage : BasePage {

        ToggleGroup<StatusExplainView> mToggleGroup = new ToggleGroup<StatusExplainView>();

        public ChangeStatusDialogPage() {
            this.InitializeComponent();
            mToggleGroup.Items.Add(statusYesView);
            mToggleGroup.Items.Add(statusNoView);
            mToggleGroup.Items.Add(statusLimitedView);
        }

        public override void OnNewParams(object args) {
            base.OnNewParams(args);
            if (args == null) {
                return;
            }

            var arg = args as ChangeStatusDialogPageArgs;

            switch (arg.status) {
                case Model.Status.YES:
                    mToggleGroup.SelectedItem = statusYesView;
                    break;
                case Model.Status.NO:
                    mToggleGroup.SelectedItem = statusNoView;
                    break;
                case Model.Status.LIMITED:
                    mToggleGroup.SelectedItem = statusLimitedView;
                    break;
                case Model.Status.UNKNOWN:
                    mToggleGroup.SelectedItem = null;
                    break;
            }

            statusYesView.IsWcStatus = arg.isInWcMode;
            statusLimitedView.IsWcStatus = arg.isInWcMode;
            statusNoView.IsWcStatus = arg.isInWcMode;

            if (arg.isInWcMode) {
                statusLimitedView.Visibility = Visibility.Collapsed;
            } else {
                statusLimitedView.Visibility = Visibility.Visible;
            }

        }

        public Model.Status GetStatus() {
            return mToggleGroup.SelectedItem?.Status ?? Model.Status.UNKNOWN;
        }

        public static void ShowInDialog(ChangeStatusDialogPageArgs args, Action<Model.Status> finishedAction) {

            var dialog = new ContentDialog() {
                Template = Application.Current.Resources["MyContentDialogControlTemplate"] as ControlTemplate,
                RequestedTheme = ElementTheme.Light
            };
            
            var frame = new Frame();
            frame.Navigate(typeof(ChangeStatusDialogPage), args);
            var page = frame.Content as ChangeStatusDialogPage;
            dialog.Content = frame;

            dialog.PrimaryButtonText = "SAVE".t(R.File.NODE);
            dialog.PrimaryButtonClick += (d, _) => {
                finishedAction?.Invoke(page.GetStatus());
            };
            dialog.ShowAsync().Forget();
        }
        
    }

    public class ChangeStatusDialogPageArgs {
        public Model.Status status;
        public bool isInWcMode;
    }
}
