﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap {
    public class Log {

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
            Debug.WriteLine($"{level} | {TAG} | {message}");
        }
    }
}
