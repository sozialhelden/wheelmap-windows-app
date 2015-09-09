using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap_Windows.Utils {

    public static class WebViewUtils {

        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

        private const int URLMON_OPTION_USERAGENT = 0x10000001;
        
        public static void SetUserAgend(string agent) {
            UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, agent, agent.Length, 0);
        }
        

    }

    public class DateUtils {
        public static long GetTimeInMilliseconds() {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = DateTime.Now;
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return (long) ts.TotalMilliseconds;
        }
    }
}
