using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Wheelmap_Windows.Extensions;
using Wheelmap_Windows.UI.Pages.Base;

namespace Wheelmap_Windows.Source.UI.Pages.Categories {
    
    public sealed partial class CategoriesListPage : BasePage {
        
        public CategoriesListPage() {
            this.InitializeComponent();            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            base.OnNavigatedTo(e);
            SetData(DataHolder.Instance.Categories.Values.ToList());
            BusProvider.DefaultInstance.Register(this);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) {
            base.OnNavigatedFrom(e);
            this.Unregister();
        }
       
        private void Item_Selected(object sender, SelectionChangedEventArgs e) {
            if (e.AddedItems.Count() <= 0) {
                return;
            }
        }
        
        private void SetData(List<Model.Category> data) {
            if (data == null) {
                return;
            }
            listView.Items.Clear();
            foreach (Model.Category n in data) {
                listView.Items.Add(n);
            }
        }
    }
}
