using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap.Utils {
    public class BulkObservableCollection<T> : ObservableCollection<T> {

        private bool _suppressNotification = false;
        private object _lock = new object();

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
            if (!_suppressNotification) {
                base.OnCollectionChanged(e);
            }
        }
        
        /**
         * calls datachange action
         * during this action no Notifications are fired
         * only one Batch task can run at once
         */
        public void CallBatch(Action action) {
            lock(_lock) {
                _suppressNotification = true;
                action.Invoke();
                _suppressNotification = false;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
        

    }
}
