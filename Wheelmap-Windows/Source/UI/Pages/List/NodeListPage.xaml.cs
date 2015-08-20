using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap_Windows.Utils;
using Wheelmap_Windows.Utils.Eventbus;
using Wheelmap_Windows.Utils.Eventbus.Events;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Wheelmap_Windows.Source.UI.Pages.List {
    
    public sealed partial class NodeListPage : Page {
        public NodeListPage() {
            this.InitializeComponent();            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            SetData(DataHolder.Instance.nodes);
            BusProvider.DefaultInstance.Register(this);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            base.OnNavigatedFrom(e);
            Unregister();
        }

        public void Unregister() {
            BusProvider.DefaultInstance.Unregister(this);
        }

        [Subscribe]
        public void OnNewData(NewNodesEvent e) {
            SetData(e?.nodes);
        }

        private void SetData(Model.Node[] data) {
            if (data == null) {
                return;
            }
            listView.Items.Clear();
            foreach (Model.Node n in data) {
                listView.Items.Add(n);
            }
        }
    }
}
