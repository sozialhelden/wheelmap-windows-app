﻿using System.Collections.Generic;
using System.Linq;
using SQLiteNetExtensions.Extensions;
using Wheelmap.Utils;
using Windows.Devices.Geolocation;

namespace Wheelmap.Model {

    public static class Nodes {

        // remember modified nodes for 10 minutes
        const long TIME_TO_DELETE_FOR_PENDING = 10 * 60 * 1000;

        public static void Save(this Node n) {
            if (n.NodeTag != NodeTag.RETRIEVED) {
                // time in ms
                n.StoreTimestamp = DateUtils.GetTimeInMilliseconds();
            }
            Database.Instance.InsertOrReplaceWithChildren(n);
        }

        public static Node QueryByWmId(long wmId) {
            var node = Database.Instance.Table<Node>().Where(x => x.wm_id == wmId).OrderBy(x => x.NodeTag).Last();
            Database.Instance.GetChildren(node);

            Geopoint point;
            if ((point = LocationManager.Instance?.LastLocationEvent?.Args?.Position?.Coordinate?.Point) != null) {
                double meters = Haversine.DistanceInMeters(Position.From(point), new Position() { Latitude = node.lat, Longitude = node.lon });
                // update distance to show on map
                node.Distance = meters;
            }
            return node;
        }

        /**
         * queries all entries but ignores doublicated nodes
         */
        public static List<Node> QueryAllDistinct() {
            var groups = from x in Database.Instance.Table<Node>() group x by x.wm_id;
            List<Node> newList = new List<Node>();
            foreach (var group in groups) {
                var node = group.OrderBy(x => x.NodeTag).Last();
                Database.Instance.GetChildren(node);
                newList.Add(node);
            }
            return newList;
        }

        public static void CleanUpOldCopies(bool force = false) {
            long now = DateUtils.GetTimeInMilliseconds();
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
        
        public static ICollection<Model.Node> OrderItemsByDistance(ICollection<Model.Node> nodes, Geopoint location) {
            if (nodes == null || location == null) {
                return nodes;
            }
            var point = Position.From(location);
            var elist = nodes.OrderBy(node => {
                double meters = Haversine.DistanceInMeters(point, new Position() { Latitude = node.lat, Longitude = node.lon });
                // update distance to show on map
                node.Distance = meters;
                return meters;
            });
            var list = elist.ToList();
            return list;
        }

        /**
         * creates a copy of the node without the _ID
         */
        public static Node CreateCopyIfNeeded(this Node n) {
            if (n.NodeTag == NodeTag.COPY) {
                return n;
            }
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
