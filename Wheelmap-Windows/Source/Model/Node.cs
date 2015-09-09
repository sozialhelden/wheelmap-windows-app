using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Utils;
using Windows.Storage;

namespace Wheelmap_Windows.Model {
    /**
     * @see related api http://wheelmap.org/api/docs/resources/nodes
     */
    public class Node : INotifyPropertyChanged {

        [PrimaryKey, AutoIncrement]
        public long _ID { get; set; }

        //---------------------------------------- API PROPERTIES ----------------------------------------//

        [JsonProperty(PropertyName = "id")]
        public long wm_id { get; set; }

        //The node's latitude in degrees as float.
        public double lat { get; set; }

        //The node's longitude in degrees as float.
        public double lon { get; set; }

        public string name { get; set; }

        [ForeignKey(typeof(NodeType))]     // Specify the foreign key
        public int nodeTypeId { get; set; }
        [JsonProperty(PropertyName = "node_type")]
        [ManyToOne]
        public NodeType nodeType { get; set; }

        [ForeignKey(typeof(Category))]     // Specify the foreign key
        public int categoryId { get; set; }
        [ManyToOne]
        public Category category { get; set; }

        //The node's wheelchair status, must be one of [yes, no, limited, unknown]
        [JsonProperty(PropertyName = "wheelchair")]
        public string wheelchairStatus { get; set; }

        //The node's additional wheelchair comment like "Ask for a ramp.", 255 characters max.
        [JsonProperty(PropertyName = "wheelchair_description")]
        public string wheelchairDescription { get; set; }

        //The node's wheelchair toilet status, must be one of [yes, no, limited, unknown]
        [JsonProperty(PropertyName = "wheelchair_toilet")]
        public string wheelchairToiletStatus { get; set; }

        //The node's street.
        public string street { get; set; }

        //The node's house number.
        public string housenumber { get; set; }

        //The node's city.
        public string city { get; set; }

        //The node's postal code.
        public string postcode { get; set; }

        //The node's url. In case there is a website about this not, for example a restaurant. Use the full url like this: http://www.example.com
        public string website { get; set; }

        //The node's phone number. In case there is a real phone number to call, for example in a restaurant. Use the international format: +49 30 123456-78
        public string phone { get; set; }


        //---------------------------------------- APP PRIVATE PROPERTIES ----------------------------------------//

        public NodeTag NodeTag { get; set; } = NodeTag.RETRIEVED;

        // indicates if this node has been changed by the user
        public DirtyState DirtyState { get; set; } = DirtyState.CLEAN;

        // the time this node has been saved
        public long StoreTimestamp { get; set; }
        
        //---------------------------------------- CALCULATED PROPERTIES ----------------------------------------//

        // calculated distance in meters
        private double _Distance = -1;
        [Ignore]
        public double Distance {
            get {
                return _Distance;
            }
            set {
                _Distance = value;
                NotifyPropertyChanged("Distance");
            }
        }

        [Ignore, JsonIgnore]
        public string DistanceString {
            get {
                if (Distance < 0) {
                    return "";
                } else if (Distance > 1000) {

                    return String.Format("{0:0.##}", (Distance / 1000)) + "km";
                } else {
                    return Convert.ToInt32(Distance) + "m";
                }
            }
        }

        [Ignore, JsonIgnore]
        public string MapIconFileUriString {
            get {
                var fileName = wheelchairStatus + "_" + (nodeType?.icon ?? ".png");
                var uri = $"ms-appdata:///local/{Constants.FOLDER_MARKER_ICONS}/{Constants.FOLDER_COMBINED_ICONS}/{fileName}";
                return uri;
            }
        }

        [Ignore, JsonIgnore]
        public Uri MapIconFileUri {
            get {
                return new Uri(MapIconFileUriString);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged(string name) {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public override string ToString() {
            return $"Node: Id={wm_id} Name={name}";
        }

        public override int GetHashCode() {
            return (int) wm_id;
        }

        public override bool Equals(object obj) {
            if ( !(obj is Node)) {
                return false;
            }
            var n = obj as Node;
            return wm_id == n.wm_id;
        }

    }

    public enum DirtyState {
        CLEAN,
        DIRTY_STATE,
        DIRTY_ALL,
    }

    public enum NodeTag {
        RETRIEVED,
        COPY,
        TEMP
    }
}
