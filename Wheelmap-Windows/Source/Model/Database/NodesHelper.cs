using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Window.Extensions;
using SQLiteNetExtensions.Extensions;

namespace Wheelmap_Windows.Model {
    
    public static class Nodes {

        // remember modified nodes for 10 minutes
        const long TIME_TO_DELETE_FOR_PENDING = 10 * 60 * 1000;

        public static void Save(this Node n) {
            if (n.NodeTag != NodeTag.RETRIEVED) {
                // time in ms
                n.StoreTimestamp = DateTime.Now.Ticks / 10000;
            }
            Database.Instance.InsertOrReplaceWithChildren(n);
        }

        public static void CleanUpOldCopies(bool force = false) {
            long now = DateTime.Now.Ticks / 10000;
            long deleteTime;
            if (!force) {
                deleteTime = now - TIME_TO_DELETE_FOR_PENDING;
            } else {
                deleteTime = now;
            }
            Database.Instance.Table<Node>().Delete(x =>  x.NodeTag != NodeTag.RETRIEVED && x.StoreTimestamp < deleteTime);
        }

        public static void DeleteRetrievedData() {
            Database.Instance.Table<Node>().Delete(x => x.NodeTag == NodeTag.RETRIEVED);
        }

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
