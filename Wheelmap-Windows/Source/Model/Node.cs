using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap_Windows.Model {

    /**
     * @see related api http://wheelmap.org/api/docs/resources/nodes
     */
    public class Node {

        public long id;

        //The node's latitude in degrees as float.
        public double lat;

        //The node's longitude in degrees as float.
        public double lon;

        public string name;

        [JsonProperty(PropertyName = "node_type")]
        public NodeType nodeType;

        public Category category;

        //The node's wheelchair status, must be one of [yes, no, limited, unknown]
        public string wheelchair;

        //The node's additional wheelchair comment like "Ask for a ramp.", 255 characters max.
        [JsonProperty(PropertyName = "wheelchair_description")]
        public string wheelchairDescription;

        //The node's street.
        public string street;

        //The node's house number.
        public string housenumber;

        //The node's city.
        public string city;

        //The node's postal code.
        public string postcode;

        //The node's url. In case there is a website about this not, for example a restaurant. Use the full url like this: http://www.example.com
        public string website;

        //The node's phone number. In case there is a real phone number to call, for example in a restaurant. Use the international format: +49 30 123456-78
        public string phone;


        public override string ToString() {
            return $"Node: Id={id} Name={name}";
        }
    }

    public class NodeType {
        long id;
        string identifier;
    }

    public class Category {
        long id;
        string identifier;
    }
}
