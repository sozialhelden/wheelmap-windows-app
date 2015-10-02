using Wheelmap.Extensions;
using Wheelmap.UI.Pages.Base;

namespace Wheelmap_Windows.Source.UI.Pages.Status {

    public sealed partial class PhoneStatusExplainPage : BasePage {

        public PhoneStatusExplainPage() {
            this.InitializeComponent();

            titleStatus.Text = "TITLE_STATUS".t();
            titleWCStatus.Text = "TITLE_WC".t();

        }

    }
}
