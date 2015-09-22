using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap.Extensions;
using Wheelmap.UI.Pages.Base;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap_Windows.Source.UI.Pages {

    public partial class WebViewPage : BasePage {

        WebViewPageArguments arguments;
        public WebViewPage() {
            this.InitializeComponent();
            
            mWebView.NavigationStarting += WebView_NavigationStarting;
            mWebView.NavigationCompleted += WebView_NavigationCompleted;
        }
        
        public override void OnNewParams(object args) {
            base.OnNewParams(args);
            if (args is WebViewPageArguments) {
                arguments = args as WebViewPageArguments;
                mWebView.Navigate(new Uri(arguments.Url));
            }
        }
        
        protected void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args) {
            mProgressBar.Visibility = Visibility.Collapsed;
        }
        
        protected void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args) {
            if (args.Uri.AbsoluteUri != arguments?.Url) {
                if ((arguments?.ShowExternalLinkInBrowser ?? false)) {
                    Launcher.LaunchUriAsync(args.Uri).Forget();
                    args.Cancel = true;
                    return;
                }
            }
            mProgressBar.Visibility = Visibility.Visible;
        }
        
    }

    public class WebViewPageArguments{
        public string Url;
        public bool ShowExternalLinkInBrowser = true;
    }
}
