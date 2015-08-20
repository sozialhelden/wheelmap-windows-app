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

namespace Wheelmap_Windows.Source.UI.Pages.Node {
    
    public sealed partial class NodeDetailPage : Page {

        public NodeDetailPage() {
            this.InitializeComponent();        
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            SetNode(e.Parameter as Model.Node);
        }

        public void SetNode(Model.Node n) {
            nameBlock.Text = n.name ?? "";
            phoneBlock.Text = n.phone ?? "";
            plzBlock.Text = n.postcode ?? "";
            streetBlock.Text = n.street ?? "";
            urlBlock.Text = n.website ?? "";
        }
    }
}
