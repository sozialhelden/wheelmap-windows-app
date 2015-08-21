using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Wheelmap_Windows.Extensions {
    public static class Extensions {

        // use this to ignore the warning to await for the result
        // for fire and forget
        public static void forget(this IAsyncAction e) {}

        public static void forget(this Task e) {}

        public static void AddAll<T>(this List<T> list, T[] items) {
            foreach(T t in items) {
                list.Add(t);
            }
        }

    }
}
