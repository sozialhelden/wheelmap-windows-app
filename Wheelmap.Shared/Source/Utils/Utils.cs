using System;
using System.Runtime.InteropServices;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;

namespace Wheelmap.Utils {

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

    public class DeviceUtils {
        public static Platform DetectPlatform() {
            bool isHardwareButtonsAPIPresent =
                ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons");

            if (isHardwareButtonsAPIPresent) {
                return Platform.WindowsPhone;
            } else {
                return Platform.Windows;
            }
        }
        
        public static int GetResolutionScaleForCurrentViewInPercentage() {
            var value = DisplayInformation.GetForCurrentView().ResolutionScale;
            switch (value) {
                case ResolutionScale.Invalid:
                    return 100;
                case ResolutionScale.Scale100Percent:
                    return 100;
                case ResolutionScale.Scale120Percent:
                    return 120;
                case ResolutionScale.Scale125Percent:
                    return 125;
                case ResolutionScale.Scale140Percent:
                    return 140;
                case ResolutionScale.Scale150Percent:
                    return 150;
                case ResolutionScale.Scale160Percent:
                    return 160;
                case ResolutionScale.Scale175Percent:
                    return 175;
                case ResolutionScale.Scale180Percent:
                    return 180;
                case ResolutionScale.Scale200Percent:
                    return 200;
                case ResolutionScale.Scale225Percent:
                    return 225;
                case ResolutionScale.Scale250Percent:
                    return 250;
                case ResolutionScale.Scale300Percent:
                    return 300;
                case ResolutionScale.Scale350Percent:
                    return 350;
                case ResolutionScale.Scale400Percent:
                    return 400;
                case ResolutionScale.Scale450Percent:
                    return 450;
                case ResolutionScale.Scale500Percent:
                    return 500;
            }
            return 100;
        }
    }
    
}
