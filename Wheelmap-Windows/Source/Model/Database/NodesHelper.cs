using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap_Windows.Model {
    public static class Nodes {

    }

    public static class NodeExtensions {

        /**
         * creates a copy of the node without the _ID
         */
        public static Node CreateCopy(this Node n) {
            return new Node {
                wm_id = n.wm_id,
                lat = n.lat,
                lon = n.lon,
                name = n.name,
                nodeType = n.nodeType,
                category = n.category,
                wheelchairStatus = n.wheelchairStatus,
                wheelchairDescription = n.wheelchairDescription,
                wheelchairToiletStatus = n.wheelchairToiletStatus,
                street = n.street,
                housenumber = n.housenumber,
                city = n.city,
                postcode = n.postcode,
                website = n.website,
                phone = n.phone,
                NodeTag = NodeTag.COPY,
                DirtyState = n.DirtyState,
                StoreTimestamp = n.StoreTimestamp,
                Distance = n.Distance,
            };
        }
    }
}
