﻿namespace Wheelmap.Utils {
    public class Constants {
        
        public const string SETTINGS_FILE_URI = "ms-appx:///Resources/Conf/settings.json";

        public const string FOLDER_MARKER_ICONS = "markericons";
        public const string FOLDER_COMBINED_ICONS = "combinedIcons";

        public const string WHEELMAP_USER_OVER_OSM = "WHEELMAP_USER_OVER_OSM";

        public static class Cortana {
            public static class PhraseList {
                public const string NODE_TYPES = "nodeTypes";
                public const string SEARCH = "search";
            }
            public static class Command {
                public const string SEARCH = "searchCommand";
                public const string HELP = "helpCommand";
                public const string WHERE_IS = "whereIsCommand";
            }
        }


        public const string USER_AGENT_FORMAT = "Wheelmap-{0}/{1} (UAP; Scale/{2})";

    }

    public class R {
        public class File {
            public const string NODE = "Node";
            public const string STATUS = "Status";
            public const string PROFILE = "Profile";
            public const string CORTANA = "CortanaVoiceCommands";
            public const string LINKS = "Links";
        }
    }
}
