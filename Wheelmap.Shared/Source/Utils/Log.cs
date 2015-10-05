using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Wheelmap {
    public class Log {
        
        public static void v(object message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0) {
            printLog("VERBOSE", message, memberName, sourceFilePath, sourceLineNumber);
        }
        
        public static void d(object message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0) {
            printLog("DEBUG", message, memberName, sourceFilePath, sourceLineNumber);
        }
        
        public static void i(object message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0) {
            printLog("INFO", message, memberName, sourceFilePath, sourceLineNumber);
        }
        
        public static void w(object message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0) {
            printLog("WARNING", message, memberName, sourceFilePath, sourceLineNumber);
        }
        
        public static void e(object message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0) {
            printLog("ERROR", message, memberName, sourceFilePath, sourceLineNumber);
        }

        private static void printLog(string level, object TAG, object message) {
#if DEBUG 
            if (message is Exception) {
                var ex = message as Exception;
                Debug.WriteLine($"{level} | {TAG} | EXCEPTION: {ex.Message}");
                Debug.WriteLine($"{level} | {TAG} | {ex.StackTrace}");
            } else if (message is ICollection) {
                var m = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                Debug.WriteLine($"{level} | {TAG} | {m}");
            } else {
                Debug.WriteLine($"{level} | {TAG} | {message}");
            }
            
#endif
        }

        private static void printLog(
            string level, 
            object message, 
            string memberName = "",
            string sourceFilePath = "",
            int sourceLineNumber = 0) {
#if DEBUG
            var TAG = System.IO.Path.GetFileName(sourceFilePath) + "#" + memberName + ":"+sourceLineNumber;
            printLog(level, TAG, message);
#endif
        }
    }
}
