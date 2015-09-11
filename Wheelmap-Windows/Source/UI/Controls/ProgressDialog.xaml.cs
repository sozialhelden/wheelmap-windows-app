using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap_Windows.Source.UI.Controls {
    public sealed partial class ProgressDialog : ContentDialog {
        
        public string Message {
            get {
                return messageTextBlock.Text;
            }
            set {
                messageTextBlock.Text = value;
            }
        }

        public bool Cancelable = false;

        public ProgressDialog() {
            this.InitializeComponent();
            Closing += ProgressDialog_Closing;
        }

        public void Close() {
            Cancelable = true;
            Hide();
        }
        
        private void ProgressDialog_Closing(ContentDialog sender, ContentDialogClosingEventArgs args) {
            args.Cancel = !Cancelable;
        }
    }
}
