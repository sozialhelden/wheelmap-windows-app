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
    class Node {

        long id;

        //The node's latitude in degrees as float.
        double lat;

        //The node's longitude in degrees as float.
        double lon;

        string name;

        [JsonProperty(PropertyName = "node_type")]
        NodeType nodeType;

        Category category;

        //The node's wheelchair status, must be one of [yes, no, limited, unknown]
        string wheelchair;

        //The node's additional wheelchair comment like "Ask for a ramp.", 255 characters max.
        [JsonProperty(PropertyName = "wheelchair_description")]
        string wheelchairDescription;

        //The node's street.
        string street;

        //The node's house number.
        string housenumber;

        //The node's city.
        string city;

        //The node's postal code.
        string postcode;

        //The node's url. In case there is a website about this not, for example a restaurant. Use the full url like this: http://www.example.com
        string website;

        //The node's phone number. In case there is a real phone number to call, for example in a restaurant. Use the international format: +49 30 123456-78
        string phone;
        
    }

    class NodeType {
        long id;
        string identifier;
    }

    class Category {
        long id;
        string identifier;
    }
}
