using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap {
    public class Log {

        public static void v(object TAG, object message) {
            printLog("VERBOSE", TAG, message);
        }

        public static void d(object TAG, object message) {
            printLog("DEBUG", TAG, message);
        }

        public static void i(object TAG, object message) {
            printLog("INFO", TAG, message);
        }

        public static void w(object TAG, object message) {
            printLog("WARNING", TAG, message);
        }
        
        public static void e(object TAG, object message) {
            printLog("ERROR", TAG, message);
        }
        
        private static void printLog(string level, object TAG, object message) {
#if DEBUG 
            if (message is Exception) {
                var ex = message as Exception;
                Debug.WriteLine($"{level} | {TAG} | EXCEPTION: {ex.Message}");
                Debug.WriteLine($"{level} | {TAG} | {ex.StackTrace}");
            } else {
                Debug.WriteLine($"{level} | {TAG} | {message}");
            }
            
#endif
        }
    }
}
