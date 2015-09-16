using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Extensions;
using Wheelmap.Utils;
using Windows.UI;

namespace Wheelmap.Model {

    public enum Status {
        YES,
        NO,
        LIMITED,
        UNKNOWN
    }

    public static class Stati {

        public static Status From(string s) {
            if (s == null) {
                return Status.UNKNOWN;
            }
            switch (s) {
                case "unknown":
                    return Status.UNKNOWN;
                case "yes":
                    return Status.YES;
                case "no":
                    return Status.NO;
                case "limited":
                    return Status.LIMITED;
                default:
                    throw new Exception("unknown type: " + s);
            }
        }

        /**
         * returns the string used bei the api to identify the status
         */
        public static string ToApiString(this Status status) {
            switch (status) {
                case Status.YES:
                    return "yes";
                case Status.NO:
                    return "no";
                case Status.LIMITED:
                    return "limited";
                case Status.UNKNOWN:
                    return "unknown";
                default:
                    throw new Exception("unknown type: " + status);
            }
        }

        public static string GetImage(this Status status) {
            switch (status) {
                case Status.YES:
                    return "ms-appx:///Assets/Images/ic_status_green.png";
                case Status.NO:
                    return "ms-appx:///Assets/Images/ic_status_red.png";
                case Status.LIMITED:
                    return "ms-appx:///Assets/Images/ic_status_orange.png";
                case Status.UNKNOWN:
                    return "ms-appx:///Assets/Images/ic_status_grey.png";
                default:
                    throw new Exception("unknown status type");
            }
        }

        public static string GetLocalizedMessage(this Status status) {
            switch (status) {
                case Status.YES:
                    return "STATUS_YES".t(R.File.STATUS);
                case Status.NO:
                    return "STATUS_NO".t(R.File.STATUS);
                case Status.LIMITED:
                    return "STATUS_LIMITED".t(R.File.STATUS);
                case Status.UNKNOWN:
                    return "STATUS_UNKNOWN".t(R.File.STATUS);
                default:
                    throw new Exception("unknown status type");
            }
        }
        
        public static string GetLocalizedToiletMessage(this Status status) {
            switch (status) {
                case Status.YES:
                    return "STATUS_TOILET_YES".t(R.File.STATUS);
                case Status.NO:
                    return "STATUS_TOILET_NO".t(R.File.STATUS);
                case Status.LIMITED:
                    return "STATUS_TOILET_LIMITED".t(R.File.STATUS);
                case Status.UNKNOWN:
                    return "STATUS_TOILET_UNKNOWN".t(R.File.STATUS);
                default:
                    throw new Exception("unknown status type");
            }
        }

        public static string[] GetHints(this Status status, bool wc) {

            int count = status.GetHintCount(wc);
            string[] hints = new string[count];

            string addForWc = wc ? "WC_" : "";
            var key = "";
            switch (status) {
                case Status.YES:
                    key = $"STATUS_{addForWc}YES_HINT_";
                    break;
                case Status.NO:
                    key = $"STATUS_{addForWc}NO_HINT_";
                    break;
                case Status.LIMITED:
                    key = $"STATUS_{addForWc}LIMITED_HINT_";
                    break;
                case Status.UNKNOWN:
                    key = $"STATUS_{addForWc}UNKNOWN_HINT_";
                    break;
            }

            for (int i=0; i<count ;i++) {
                hints[i] = (key + i).t(R.File.STATUS);
            }

            return hints;
        }

        public static int GetHintCount(this Status status, bool wc) {
            string addForWc = wc ? "WC_" : "";
            string countKey = "";
            switch (status) {
                case Status.YES:
                    countKey = $"STATUS_{addForWc}YES_HINT_COUNT";
                    break;
                case Status.NO:
                    countKey = $"STATUS_{addForWc}NO_HINT_COUNT";
                    break;
                case Status.LIMITED:
                    countKey = $"STATUS_{addForWc}LIMITED_HINT_COUNT";
                    break;
                case Status.UNKNOWN:
                    countKey = $"STATUS_{addForWc}UNKNOWN_HINT_COUNT";
                    break;
            }
            var countAsString = countKey.t(R.File.STATUS);
            try {
                return Int32.Parse(countAsString);
            } catch {
                return 0;
            }
        }

        public static Color GetColor(this Status status) {
            switch (status) {
                case Status.YES:
                    // #99c15c
                    return Color.FromArgb(0xff, 0x99, 0xc1, 0x5c);
                case Status.NO:
                    // #d23b3a
                    return Color.FromArgb(0xff, 0xd2, 0x3b, 0x3a);
                case Status.LIMITED:
                    // #ed9b56 
                    return Color.FromArgb(0xff, 0xed, 0x9b, 0x56);
                case Status.UNKNOWN:
                    // #8b8c8d
                    return Color.FromArgb(0xff, 0x8b, 0x8c, 0x8d);
                default:
                    throw new Exception("unknown status type");
            }
        }
    }
}
