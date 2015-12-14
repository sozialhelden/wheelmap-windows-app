using System;
using System.Linq;
using System.Threading.Tasks;
using Wheelmap.Api.Calls;
using Wheelmap.UI.Pages.Base;
using Wheelmap.Utils.Preferences;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Wheelmap.Extensions;
using Wheelmap.Utils;
using Wheelmap.Model;

namespace Wheelmap.Source.UI.Pages.Profile {

    public sealed partial class LoginPage : BasePage {
        
        bool skip = false;
        
        public LoginPage() {
            this.InitializeComponent();
            InitWebView();
        }

        private void InitWebView() {
            var url = BuildConfig.API_BASEURL + ApiConstants.WEB_LOGIN_LINK;
            mWebView.Navigate(new Uri(url));
            mWebView.NavigationStarting += WebView_NavigationStarting;
            mWebView.UnsupportedUriSchemeIdentified += WebView_UnsupportedUriSchemeIdentified;
            mWebView.NavigationCompleted += WebView_NavigationCompleted;
        }

        private void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args) {
            mProgressBar.Visibility = Visibility.Collapsed;
        }
        
        private void WebView_UnsupportedUriSchemeIdentified(WebView sender, WebViewUnsupportedUriSchemeIdentifiedEventArgs args) {

            if (args.Uri.Scheme.Equals("wheelmap")) {
                args.Handled = true;

                User user = new User();
                user.email = null;
                user.apiKey = null;
                user.privacyAccepted = false;
                user.termsAccepted = false;
                
                var paramList = args.Uri.ToString().Split('?').Last().Split('&');
                foreach(string p in paramList) {
                    var keyVal = p.Split('=');
                    if (keyVal.Length > 1) {
                        var key = keyVal[0];
                        var value = keyVal[1];
                        switch (key) {
                            case "token":
                                user.apiKey = value;
                                break;
                            case "email":
                                user.email = value;
                                break;
                            case "privacy_accepted":
                                user.privacyAccepted = Convert.ToBoolean(value);
                                break;
                            case "terms_accepted":
                                user.termsAccepted = Convert.ToBoolean(value);
                                break;
                        }
                    }
                }

                User.CurrentUser = user;
                
            }


            Log.d(args.Uri);
        }
        
        private void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) {

            mProgressBar.Visibility = Visibility.Visible;

            Log.d(args.Uri);
            if (args.Uri.ToString().Contains("facebook.com")) {
                args.Cancel = true;
                return;
            }

            if (!skip && args.Uri.ToString().Contains("/users/auth/osm/callback")) {
                skip = true;
                args.Cancel = true;

                Task.Run(() => {
                    Dispatcher.RunAsync(CoreDispatcherPriority.Normal,() => {
                        HttpRequestMessage request = new HttpRequestMessage();
                        request.RequestUri = args.Uri;
                        request.Headers.Add("Install-ID", Prefs.InstallId);
                        // use a custom user agent to change the behavior of the website
                        request.Headers.Add("User-Agent", WebViewUtils.CreateAppsUserAgent());
                        mWebView.NavigateWithHttpRequestMessage(request);
                    }).Forget();
                });
            }
            
        }
    }
}
