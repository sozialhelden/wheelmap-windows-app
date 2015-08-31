using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Extensions;
using Windows.UI;

namespace Wheelmap_Windows.Model {
    public enum Status {
        YES,
        NO,
        LIMITED,
        UNKNOWN
    }

    public static class Stati {

        public static Status From(string s) {
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

        public static string GetImage(this Status status) {
            switch (status) {
                case Status.YES:
                    return "ms-appx:///Assets/Images/ic_status_green.png";
                case Status.NO:
                    return "ms-appx:///Assets/Images/ic_status_red.png";
                case Status.LIMITED:
                    return "ms-appx:///Assets/Images/ic_status_orange.png";
                case Status.UNKNOWN:
                    return "ms-appx:///Assets/Images/ic_unknown.png";
                default:
                    throw new Exception("unknown status type");
            }
        }

        public static string GetLocalizedMessage(this Status status) {
            switch (status) {
                case Status.YES:
                    return "STATUS_YES".t();
                case Status.NO:
                    return "STATUS_NO".t();
                case Status.LIMITED:
                    return "STATUS_LIMITED".t();
                case Status.UNKNOWN:
                    return "STATUS_UNKNOWN".t();
                default:
                    throw new Exception("unknown status type");
            }
        }
        
        public static string GetLocalizedToiletMessage(this Status status) {
            switch (status) {
                case Status.YES:
                    return "STATUS_TOILET_YES".t();
                case Status.NO:
                    return "STATUS_TOILET_NO".t();
                case Status.LIMITED:
                    return "STATUS_TOILET_LIMITED".t();
                case Status.UNKNOWN:
                    return "STATUS_TOILET_UNKNOWN".t();
                default:
                    throw new Exception("unknown status type");
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
