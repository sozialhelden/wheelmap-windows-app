using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Source.Utils.Interfaces;
using Windows.Foundation;

namespace Wheelmap_Windows.Utils {

    /**
     * manages a collection of items and their selected state
     * uses delegate or Selectable to change the state
     */
    public class ToggleGroup<T> {

        public delegate void ChangeState(T item, bool selected);

        public event TypedEventHandler<T, bool> StateChanged;
        
        private T _SelectedItem;
        public T SelectedItem {
            set {
                if ((object) _SelectedItem == (object) value) {
                    return;
                }
                if (_SelectedItem is Selectable) {
                    (_SelectedItem as Selectable).Selected = false;
                }
                StateChanged?.Invoke(_SelectedItem, false);
                _SelectedItem = value;

                if (_SelectedItem is Selectable) {
                    (_SelectedItem as Selectable).Selected = true;
                }
                StateChanged?.Invoke(_SelectedItem, true);
            }
            get {
                return _SelectedItem;
            }
        }

        public ObservableCollection<T> Items = new ObservableCollection<T>();

        public ToggleGroup() {
            Items.CollectionChanged += Items_CollectionChanged;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Remove) {
                foreach (T item in e.OldItems) {
                    if (item is Selectable) {
                        //Removed items
                        (item as Selectable).PropertyChanged -= ToggleGroup_PropertyChanged;
                    }
                }
            } else if (e.Action == NotifyCollectionChangedAction.Add) {
                foreach (T item in e.NewItems) {
                    //Added items
                    if (item is Selectable) {
                        //Removed items
                        (item as Selectable).PropertyChanged += ToggleGroup_PropertyChanged; ;
                    }
                }
            } else {
                foreach (T item in e.OldItems) {
                    if (item is Selectable) {
                        //Removed items
                        (item as Selectable).PropertyChanged -= ToggleGroup_PropertyChanged;
                    }
                }
                foreach (T item in e.NewItems) {
                    //Added items
                    if (item is Selectable) {
                        //Removed items
                        (item as Selectable).PropertyChanged += ToggleGroup_PropertyChanged; ;
                    }
                }
            }
        }

        private void ToggleGroup_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (sender is Selectable) {
                if ((sender as Selectable).Selected) {
                    SelectedItem = (T)sender;
                }
            }
        }
    }
}
