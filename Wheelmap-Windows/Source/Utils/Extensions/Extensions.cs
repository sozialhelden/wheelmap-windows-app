using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Utils.Eventbus;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Wheelmap_Windows.Extensions {
    public static class Extensions {

        // use this to ignore the warning to await for the result
        // for fire and forget
        public static void forget(this IAsyncAction e) { }

        public static void forget(this Task e) { }

        public static void ContinueWithOnMainThread<T>(this Task<T> e, Action<Task<T>> continuationAction) {
            e.ContinueWith((task) => {
                CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    continuationAction.Invoke(task);
                }).forget();
            });
        }

        public static void AddAll<X, T>(this ICollection<X> list, T[] items) where T : X {
            if (items == null) {
                return;
            }
            foreach (T t in items) {
                list.Add(t);
            }
        }

        public static void Unregister(this Page page) {
            BusProvider.DefaultInstance.Unregister(page);
        }

    }

}
