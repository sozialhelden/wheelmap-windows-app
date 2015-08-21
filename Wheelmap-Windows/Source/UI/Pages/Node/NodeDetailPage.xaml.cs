using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Wheelmap_Windows.Api.Calls;
using Wheelmap_Windows.Extensions;
using Wheelmap_Windows.Model;
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

            try {
                phoneButton.NavigateUri = new Uri("tel:"+n.phone);
                phoneButton.Content = n.phone;
                phoneButton.Visibility = Visibility.Visible;
            } catch {
                phoneButton.Visibility = Visibility.Collapsed;
            }
            
            plzBlock.Text = n.postcode ?? "";
            streetBlock.Text = n.street ?? "";
            try {
                websideButton.NavigateUri = new Uri(n.website);
                websideButton.Content = n.website;
                websideButton.Visibility = Visibility.Visible;
            } catch {
                websideButton.Visibility = Visibility.Collapsed;
            }

            new PhotosRequest(n).Query().ContinueWithOnMainThread((photos) => {
                listView.Items.Clear();
                Log.d(this,"Photos: "+ photos.Result.Count());
                foreach (Photo p in photos.Result) {
                    Log.d(this,p.GetThumb());
                    listView.Items.Add(p);
                }
            });

        }
    }
    
}
