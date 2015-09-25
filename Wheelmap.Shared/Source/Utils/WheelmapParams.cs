using Newtonsoft.Json;
using System.Collections.Generic;
using Wheelmap.Model;

namespace Wheelmap.Utils {
    public class WheelmapParams {

        public const string PATH_HELP = "help";

        public string Path;

        /// <summary>
        /// all stati which should be filted out
        /// </summary>
        public List<Status> WheelmapStati = new List<Status>();

        /// <summary>
        /// all stati which should be filted out
        /// </summary>
        public List<Status> WheelmapToiletStati = new List<Status>();

        /// <summary>
        /// search string
        /// </summary>
        public string Search;

        /// <summary>
        /// wheelmap id of the node which should be shown on detail screen
        /// </summary>
        public long ShowDetailsFromId;
        
        public override string ToString() {
            return JsonConvert.SerializeObject(this);
        }

        public static WheelmapParams FromString(string s) {
            return JsonConvert.DeserializeObject<WheelmapParams>(s);
        }
    
    }
}
