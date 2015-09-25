using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap.Extensions;
using Wheelmap.UI.Pages.Base;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap_Windows.Source.UI.Pages.Status {

    public sealed partial class PhoneStatusExplainPage : BasePage {

        public PhoneStatusExplainPage() {
            this.InitializeComponent();

            titleStatus.Text = "TITLE_STATUS".t();
            titleWCStatus.Text = "TITLE_WC".t();

        }

    }
}
