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
using System.ComponentModel;

namespace Wheelmap_Windows.Source.UI.Pages.Categories {
    
    public sealed partial class CategoriesListPage : BasePage {

        BulkObservableCollection<CategoryListModel> mItems = new BulkObservableCollection<CategoryListModel>();

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

    class CategoryListModel : INotifyPropertyChanged {

        private Model.Category _category;
        public Model.Category Category {
            get {
                return _category;
            }
            set {
                _category = value;
                Selected = !DataHolder.Instance.Filter.FilteredCategoryIdntifier.Contains(value.identifier);
                NotifyPropertyChanged(nameof(Category));
            }
        }

        private bool _selected;
        public bool Selected {
            get {
                return _selected;
            }
            set {
                if (_selected == value) {
                    return;
                }
                _selected = value;
                NotifyPropertyChanged(nameof(Selected));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        
    }
}
