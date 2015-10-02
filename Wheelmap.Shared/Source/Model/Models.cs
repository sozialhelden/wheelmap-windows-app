using Newtonsoft.Json;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System.Linq;
using Wheelmap.Utils;

namespace Wheelmap.Model {

    public class NodeType {

        [JsonProperty(PropertyName = "id")]
        [PrimaryKey]
        public int Id { get; set; }

        public int category_id { get; set; }

        [OneToMany]
        public Category category { get; set; }

        private string _localizedName;

        [JsonProperty(PropertyName = "localized_name")]
        public string localizedName {
            get {
                if (_localizedName != null) {
                    return _localizedName;
                }
                if (DataHolder.Instance.NodeTypeById.ContainsKey(Id)) {
                    return DataHolder.Instance.NodeTypeById[Id].localizedName;
                }
                return identifier;
            }
            set {
                _localizedName = value;
            }
        }

        public string _icon;
        // for example boat.png
        public string icon {
            get {
                if (_icon != null) {
                    return _icon;
                }
                if (DataHolder.Instance.NodeTypeById.ContainsKey(Id)) {
                    var type = DataHolder.Instance.NodeTypeById[Id];
                    if (type == this) {
                        return null;
                    }
                    return type.icon;
                }
                return null;
            }
            set {
                _icon = value;
            }
        }

        public string identifier { get; set; }
    }

    public class Category {

        [JsonProperty(PropertyName = "id")]
        [PrimaryKey]
        public int Id { get; set; }
        public string identifier { get; set; }

        private string _localizedName;

        [JsonProperty(PropertyName = "localized_name")]
        public string localizedName {
            get {
                if (_localizedName != null) {
                    return _localizedName;
                }
                if (DataHolder.Instance.Categories.ContainsKey(identifier)) {
                    return DataHolder.Instance.Categories[identifier].localizedName;
                }
                return identifier;
            }
            set {
                _localizedName = value;
            }
        }
    }

    /**
     * http://wheelmap.org/api/docs/resources/photos
     */
    public class Photo {
        public int id { get; set; }

        [JsonProperty(PropertyName = "taken_on")]
        public long takenOnTimeStamp { get; set; }

        public Image[] images;

        public Image Thumb {
            get {
                return GetThumb();
            }
        }

        public Image Source {
            get {
                return GetSource();
            }
        }

        public Image GetThumb() {
            return GetImage("thumb_iphone_retina");
        }

        public Image GetSource() {
            return GetImage("gallery_iphone_retina");
        }

        /**
         * searches the given image type and falls back to the original image if not found
         * returns null if no images available
         */
        public Image GetImage(string type) {
            if (images == null || images.Count() <= 0) {
                return null;
            }

            Image original = images.First();

            foreach (Image i in images) {
                if (i.type == "original") {
                    original = i;
                }
                if (i.type == type) {
                    return i;
                }
            }
            
            return original;
        }

        public override string ToString() {
            return "{Id: " + id+ ", Source: "+GetSource().url+" }";
        }
    }

    public class Image {

        //for example: original, gallery, gallery_iphone, gallery_iphone_retina, gallery_ipad, gallery_ipad_retina, thumb, thumb_iphone, thumb_iphone_retina
        public string type { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public string url { get; set; }

        public override string ToString() {
            return "Image: " + url;
        }
    }

    public class Asset {
        public int id { get; set; }

        // marker, icons, icons_white
        [JsonProperty(PropertyName = "name")]
        public string type { get; set; }

        public string url { get; set; }
        public string license { get; set; }
        public long modified_at { get; set; }
    }

}
