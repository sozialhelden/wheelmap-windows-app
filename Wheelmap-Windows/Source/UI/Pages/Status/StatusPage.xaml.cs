using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Wheelmap_Windows.Source.UI.Pages.Status {
    
    public sealed partial class StatusPage : BasePage {

        public StatusPage() {
            this.InitializeComponent();
            WindowSizeStates.CurrentStateChanged += (sender, e) => OnStateChanged(WindowSizeStates.CurrentState);
            OnStateChanged(WindowSizeStates.CurrentState);
        }
        
        private void OnStateChanged(VisualState state) {
            var showHints = state != STATE_SMALL;
            statusYesView.ShowStatusHints = showHints;
            statusLimitedView.ShowStatusHints = showHints;
            statusNoView.ShowStatusHints = showHints;
            statusUnknownView.ShowStatusHints = showHints;
        }
    }
}
