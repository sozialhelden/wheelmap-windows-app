using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Windows.ApplicationModel;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;

namespace Wheelmap.Utils {

    public static class WebViewUtils {

        [DllImport("urlmon.dll", CharSet = CharSet.Ansi, ExactSpelling = true)]
        private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

        private const int URLMON_OPTION_USERAGENT = 0x10000001;
        
        public static void SetUserAgent(string agent) {
            try { 
                UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, agent, agent.Length, 0);
            }catch { }
        }

        public static string CreateAppsUserAgent() {
            var format = Constants.USER_AGENT_FORMAT;
            string appVersion = string.Format("{0}.{1}.{2}.{3}",
                    Package.Current.Id.Version.Major,
                    Package.Current.Id.Version.Minor,
                    Package.Current.Id.Version.Build,
                    Package.Current.Id.Version.Revision);
            var userAgent = String.Format(format,
                BuildConfig.BUILDTYPE,
                appVersion,
                (DeviceUtils.GetResolutionScaleForCurrentViewInPercentage()/100d).ToString(CultureInfo.InvariantCulture));
            return userAgent;
        }

        public static void SetUserAgentForApp() {
            var userAgent = CreateAppsUserAgent();
            Log.d(userAgent);
            SetUserAgent(userAgent);
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
