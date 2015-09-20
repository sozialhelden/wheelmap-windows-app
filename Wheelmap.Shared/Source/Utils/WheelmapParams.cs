using Newtonsoft.Json;
using System.Collections.Generic;
using Wheelmap.Model;

namespace Wheelmap.Utils {
    public class WheelmapParams {

        private const string PARAM_SEARCH = "search";
        private const string PARAM_DETAILS = "details";
        private const string PARAM_STATI = "stati";
        private const string PARAM_TOILET_STATI = "toiletStati";

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
