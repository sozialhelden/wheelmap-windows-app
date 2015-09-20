using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap.Extensions;
using Wheelmap.UI.Pages.Base;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap_Windows.Source.UI.Pages.Settings {

    public sealed partial class SettingsPage : BasePage {

        public override string Title {
            get {
                return "TITLE_SETTINGS".t().ToUpper();
            }
        }

        public SettingsPage() {
            this.InitializeComponent();
            titleTextBlock.Text = Title ?? "";
            
            string appVersion = string.Format("Version: {0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);
            versionTextBlock.Text = appVersion;
        }
    }
}
