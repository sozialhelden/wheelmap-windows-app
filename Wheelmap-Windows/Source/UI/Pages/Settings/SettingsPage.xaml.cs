using Wheelmap.Extensions;
using Wheelmap.UI.Pages.Base;
using Windows.ApplicationModel;

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
