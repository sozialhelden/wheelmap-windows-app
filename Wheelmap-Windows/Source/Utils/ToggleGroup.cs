using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Wheelmap_Windows.Utils {

    /**
     * manages a collection of items and their selected state
     * uses delegate to change the state
     */
    public class ToggleGroup<T> {

        public delegate void ChangeState(T item, bool selected);
        public event TypedEventHandler<T, bool> StateChanged;

        private T _SelectedItem;
        public T SelectedItem {
            set {
                StateChanged?.Invoke(_SelectedItem, false);
                _SelectedItem = value;
                StateChanged?.Invoke(_SelectedItem, true);
            }
            get {
                return _SelectedItem;
            }
        }

        public List<T> Items = new List<T>();
        
    }
}
