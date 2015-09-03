using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Extensions;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Wheelmap_Windows.Utils.Eventbus {

    /**
     * https://github.com/snotyak/EventBus with minor changes
     */
    public class EventBus {

        const string TAG = nameof(EventBus);

        private List<WeakReference> _subscribers = new List<WeakReference>();
        
        public EventBus() {

        }

        public void Register(object subscriber) {
            Log.i(TAG, "Register: " + subscriber);
            lock (_subscribers) {
                if (!_subscribers.ContainsWeak(subscriber)) {
                    _subscribers.Add(new WeakReference(subscriber));
                    _subscribers.RemoveAll((item) => !item.IsAlive);
                }
            }
            Log.i(TAG, "Register Count: " + _subscribers.Count());
        }

        public void Unregister(object subscriber) {
            Log.i(TAG, "Unregister: " + subscriber);
            lock(_subscribers) {
                _subscribers.RemoveWeak(subscriber);
                _subscribers.RemoveAll((item) => !item.IsAlive);
            }
        }

        public void Post(object e) {

            // run callback on mainthread
            CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => {
                    lock (_subscribers) {
                        foreach (WeakReference instance in _subscribers) {
                            if (!instance.IsAlive) {
                                continue;
                            }
                            var sub = instance.Target;
                            foreach (MethodInfo method in GetSubscribedMethods(sub.GetType(), e)) {
                                try {
                                    method?.Invoke(sub, new object[] { e });
                                } catch (TargetInvocationException) { }
                            }
                        }
                    }
                }
            ).Forget();

        }
        
        private List<MethodInfo> GetSubscribedMethods(Type type, object obj) {
            List<MethodInfo> subscribedMethods = new List<MethodInfo>();

            var methods = type.GetRuntimeMethods();
            foreach (MethodInfo info in methods) {
                foreach (Attribute attr in info.GetCustomAttributes()) {
                    if (attr.GetType() == typeof(Subscribe)) {
                        var paramInfo = info.GetParameters();
                        if (paramInfo.Length == 1 && paramInfo[0].ParameterType == obj.GetType()) {
                            subscribedMethods.Add(info);
                        }
                    }
                }
            }

            return subscribedMethods;
        }
    }
}
