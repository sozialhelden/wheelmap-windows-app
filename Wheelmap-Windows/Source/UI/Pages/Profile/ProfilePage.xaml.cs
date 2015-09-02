using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap_Windows.Api.Calls;
using Wheelmap_Windows.UI.Pages.Base;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace Wheelmap_Windows.Source.UI.Pages.Profile {
    
    public sealed partial class ProfilePage : BasePage {
        public ProfilePage() {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            new LoginRequest("test", "test").Query().ContinueWith((task)=> {
                var user = task.Result;
            });
        }
    }
}
