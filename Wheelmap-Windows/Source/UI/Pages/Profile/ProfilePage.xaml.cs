using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap.Api.Calls;
using Wheelmap.Extensions;
using Wheelmap.Model;
using Wheelmap.UI.Pages.Base;
using Wheelmap.Utils;
using Wheelmap.Utils.Eventbus;
using Wheelmap.Utils.Eventbus.Events;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Wheelmap.Source.UI.Pages.Profile {
    
    public sealed partial class ProfilePage : BasePage {

        public ProfilePage() {
            this.InitializeComponent();
            UserChanged(new UserChangedEvent { User = User.CurrentUser });
            BusProvider.DefaultInstance.Register(this);
            InitLocalizations();
            loginPanelScrollViewer.HideVerticalScrollBarsIfContentFits();
        }

        private void InitLocalizations() {
            LoginButton.Text = "PROFILE_LOGIN".t(R.File.PROFILE);
            hitLoginTextBlock.Text = "PROFILE_HINT_LOGIN".t(R.File.PROFILE);
            noOSMAccountTextBlock.Text = "PROFILE_HINT_NO_ACCOUNT_TITLE".t(R.File.PROFILE);
            stepsToOsmAccountTextBlock.Text = "StepsToOSMAccount".t(R.File.PROFILE);
            registerButton.Text = "OSMRegistration".t(R.File.PROFILE);
        }
        
        [Subscribe]
        private void UserChanged(UserChangedEvent e) {
            if (e?.User == null) {
                loggedInPanel.Visibility = Visibility.Collapsed;
                loginPanel.Visibility = Visibility.Visible;
            } else {
                loggedInPanel.Visibility = Visibility.Visible;
                loginPanel.Visibility = Visibility.Collapsed;

                if (e?.User?.email == null) {
                    loggedInTextBlock.Text = "SignedIn".t(R.File.PROFILE);
                } else {
                    loggedInTextBlock.Text = String.Format("SignedInAs".t(R.File.PROFILE), e.User.email);
                }

            }
        }
        
        private void Logout_Tapped(object sender, TappedRoutedEventArgs e) {
            User.CurrentUser = null;
        }

        private void Login_Tapped(object sender, TappedRoutedEventArgs e) {
            ShowOnDetailFrame(typeof(LoginPage));
        }

        private void Register_Tapped(object sender, TappedRoutedEventArgs e) {
            Windows.System.Launcher.LaunchUriAsync(new Uri(BuildConfig.API_BASEURL+ApiConstants.WM_REGISTER_LINK)).Forget();
        }
    }
}
