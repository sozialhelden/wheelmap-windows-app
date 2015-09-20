﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap.Utils {
    public class Constants {
        public const string FOLDER_MARKER_ICONS = "markericons";
        public const string FOLDER_COMBINED_ICONS = "combinedIcons";

        public const string WHEELMAP_USER_OVER_OSM = "WHEELMAP_USER_OVER_OSM";

        public static class Cortana {
            public static class PhraseList {
                public const string NODE_TYPES = "nodeTypes";
                public const string SEARCH = "dictatedSearchTerms";
            }
        }

    }

    public class R {
        public class File {
            public const string NODE = "Node";
            public const string STATUS = "Status";
            public const string PROFILE = "Profile";
            public const string CORTANA = "CortanaVoiceCommands";
        }
    }
}