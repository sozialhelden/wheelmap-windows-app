using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Wheelmap.Utils.Eventbus;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Wheelmap.Extensions {
    public static class Extensions {
        
        // use this to ignore the warning to await for the result
        // for fire and forget
        public static void Forget(this IAsyncAction e) { }
        public static void Forget<T>(this IAsyncOperation<T> e) { }
        public static void Forget(this Task e) { }

        public static void ContinueWithOnDispatcher<T>(this Task<T> e, CoreDispatcher dispatcher, Action<Task<T>> continuationAction, long minTime = 0) {
            long time = DateTime.Now.Ticks / 10000;
            e.ContinueWith((task) => {
                long timeDif = (DateTime.Now.Ticks / 10000) - time;
                if (timeDif < minTime) {
                    Task.Delay((int) (minTime - timeDif)).Wait();
                }
                dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                    continuationAction.Invoke(task);
                }).Forget();
            });
        }

        public static async void PostDelayed(this CoreDispatcher dispatcher, DispatchedHandler action, int delayInMilliseconds) {
            await Task.Delay(delayInMilliseconds);
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, action); 
        }

        internal struct VoidTypeStruct { }
        public static Task TimeoutAfter(this Task task, int millisecondsTimeout) {
            // tcs.Task will be returned as a proxy to the caller
            TaskCompletionSource<VoidTypeStruct> tcs =
                new TaskCompletionSource<VoidTypeStruct>();

            // Set up a timer to complete after the specified timeout period
            Timer timer = new Timer(_ =>
            {
                // Fault our proxy Task with a TimeoutException
                tcs.TrySetException(new TimeoutException());
            }, null, millisecondsTimeout, Timeout.Infinite);

            // Wire up the logic for what happens when source task completes
            task.ContinueWith(antecedent =>
            {
                timer.Dispose(); // Cancel the timer
                MarshalTaskResults(antecedent, tcs); // Marshal results to proxy
            }, CancellationToken.None,
                TaskContinuationOptions.ExecuteSynchronously,
                TaskScheduler.Default);

            return tcs.Task;
        }

        internal static void MarshalTaskResults<TResult>( Task source, TaskCompletionSource<TResult> proxy) {
            switch (source.Status) {
                case TaskStatus.Faulted:
                    proxy.TrySetException(source.Exception);
                    break;
                case TaskStatus.Canceled:
                    proxy.TrySetCanceled();
                    break;
                case TaskStatus.RanToCompletion:
                    Task<TResult> castedSource = source as Task<TResult>;
                    proxy.TrySetResult(
                        castedSource == null ? default(TResult) : // source is a Task
                            castedSource.Result); // source is a Task<TResult>
                    break;
            }
        }

        public static void AddAll<X, T>(this ICollection<X> list, T[] items) where T : X {
            if (items == null) {
                return;
            }
            foreach (T t in items) {
                list.Add(t);
            }
        }

        public static void AddAll<X, T>(this ICollection<X> list, ICollection<T> items) where T : X {
            if (items == null) {
                return;
            }
            foreach (T t in items) {
                list.Add(t);
            }
        }

        public static bool ContainsWeak(this ICollection<WeakReference> list, object item) {
            foreach (WeakReference r in list) {
                if (r.IsAlive && r.Target == item) {
                    return true;
                }
            }
            return false;
        }

        public static bool RemoveWeak(this ICollection<WeakReference> list, object item) {
            WeakReference remove = null;
            foreach (WeakReference r in list) {
                if (r.IsAlive && r.Target == item) {
                    remove = r;
                }
            }
            if (remove != null) {
                return list.Remove(remove);
            }
            return false;
        }

        /**
         * calls an task and wait for it
         */
        public static Task<T> Sync<T>(this Task<T> task) {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);
            
            Task<T> result = null;
            task.ContinueWith((t) => {
                result = t;
                autoResetEvent.Set();
            });

            // Wait until the call is finished
            autoResetEvent.WaitOne();

            return result;
        }

        public static void Unregister(this Page page) {
            BusProvider.DefaultInstance.Unregister(page);
        }
        
    }


    public static class TypeExtensions {

        public static object GetNewObject(this Type t) {
            try {
                return t.GetConstructor(new Type[] { }).Invoke(new object[] { });
            }
            catch {
                return null;
            }
        }
    }

}
