using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap_Windows.Utils {

    /**
     * manages a collection of items and their selected state
     * uses delegate to change the state
     */
    public class ToggleGroup<T> {

        public delegate void ChangeState(T item, bool selected);

        private T _SelectedItem;
        public T SelectedItem {
            set {
                Delegate?.Invoke(_SelectedItem, false);
                _SelectedItem = value;
                Delegate?.Invoke(_SelectedItem, true);
            }
            get {
                return _SelectedItem;
            }
        }

        public List<T> Items = new List<T>();

        public ChangeState Delegate;
        
    }
}
