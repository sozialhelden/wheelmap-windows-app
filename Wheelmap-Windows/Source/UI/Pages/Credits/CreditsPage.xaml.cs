using Wheelmap.Extensions;
using Wheelmap.UI.Pages.Base;
using Windows.ApplicationModel;

namespace Wheelmap.Source.UI.Pages.Credits {

    public sealed partial class CreditsPage : BasePage {
        public CreditsPage() {
            this.InitializeComponent();

            creditsMicrosoftTextBlock.Text = "CREDITS_MICROSOFT".t();
            string appVersion = string.Format("Version: {0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);
            versionTextBlock.Text = appVersion;
        }
    }
}
