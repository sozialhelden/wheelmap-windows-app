﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Utils;

namespace Wheelmap_Windows.Model {

    /**
     * @see related api http://wheelmap.org/api/docs/resources/nodes
     */
    public class Node {

        public long id { get; set; }

        //The node's latitude in degrees as float.
        public double lat { get; set; }

        //The node's longitude in degrees as float.
        public double lon { get; set; }

        public string name { get; set; }

        [JsonProperty(PropertyName = "node_type")]
        public NodeType nodeType { get; set; }

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


        public override string ToString() {
            return $"Node: Id={id} Name={name}";
        }
    }

    public class NodeType {
        public long id { get; set; }
        public string identifier { get; set; }
    }

    public class Category {
        public long id { get; set; }
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

}
