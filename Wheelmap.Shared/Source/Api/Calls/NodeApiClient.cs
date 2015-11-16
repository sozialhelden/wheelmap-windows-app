using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Wheelmap.Api.Model;
using Wheelmap.Model;
using Wheelmap.Source.Utils.Threads;
using Windows.Devices.Geolocation;
using SQLiteNetExtensions.Extensions;
using Wheelmap.Utils;

/// <summary>
/// contains all methods to query Node from the Wheelmap Api
/// @see http://wheelmap.org/api/docs/resources/nodes 
/// </summary>
namespace Wheelmap.Api.Calls {

    public class NodesRequest : PagedRequest<NodesResponse, Node> {

        // only one NodeRequest should run at a time
        private static LimitedConcurrencyLevelTaskScheduler lcts = new LimitedConcurrencyLevelTaskScheduler(1);
        private static TaskFactory factory = new TaskFactory(lcts);

        protected GeoboundingBox bbox;

        public NodesRequest(GeoboundingBox bbox, bool singlePage = false) : base(factory, singlePage) {
            this.bbox = bbox;
        }

        public override Task<List<Node>> Execute() {
            // tell scheduler to skip all task in queue
            // they are all outdated now
            lcts.ClearTasks();
            return base.Execute();
        }

        protected override Task<List<Node>> prepareData(List<Node> items) {
            if (items == null) {
                return null;
            }

            List<Node> newList = items;
            Database.Instance.RunInTransaction(() => {
                // remove all old or outdated data
                Nodes.CleanUpOldCopies();

                Nodes.DeleteRetrievedData();

                // insert all nodes to database
                Database.Instance.InsertAllWithChildren(items);

                // query all notes from database to also get all cached data
                newList = Nodes.QueryAllDistinct();

            });
            
            return Task.FromResult(newList);
        }

        protected override List<Node> QueryPages() {
            List<Node> result = null;
            DataHolder.Instance.IsRequestRunning = true;
            try {
                result = base.QueryPages();
            } catch {}
            DataHolder.Instance.IsRequestRunning = false;
            return result;
        }

        protected override string GetUrl(int page) {
            string pageParam = "page=" + page.ToString(CultureInfo.InvariantCulture);
            string pageSizeParam = "per_page=" + PAGE_SIZE.ToString(CultureInfo.InvariantCulture);

            string bboxParam  = "bbox="
                    + bbox.NorthwestCorner.Longitude.ToString(CultureInfo.InvariantCulture) + ","
                    + bbox.NorthwestCorner.Latitude.ToString(CultureInfo.InvariantCulture) + ","
                    + bbox.SoutheastCorner.Longitude.ToString(CultureInfo.InvariantCulture) + ","
                    + bbox.SoutheastCorner.Latitude.ToString(CultureInfo.InvariantCulture);
            
            string url = BuildConfig.API_BASEURL + ApiConstants.END_POINT_NODES + "?"
                + ApiConstants.API_KEY_PARAM + "&"
                + bboxParam + "&"
                + pageSizeParam + "&"
                + pageParam;
            
            return url;
        }

    }

    public class NodeSearchRequest : NodesRequest {

        private string queryString;
        public NodeSearchRequest(String query) : base(null) {
            queryString = query;
        }
        
        protected override string GetUrl(int page) {
            string pageParam = "page=" + page;
            string pageSizeParam = "page_size=" + PAGE_SIZE;
            string searchParam = "q=" + Uri.EscapeDataString(queryString);

            string bboxParam = null;
            if (bbox != null) {
                bboxParam = "bbox="
                    + bbox.NorthwestCorner.Longitude.ToString(CultureInfo.InvariantCulture) + ","
                    + bbox.NorthwestCorner.Latitude.ToString(CultureInfo.InvariantCulture) + ","
                    + bbox.SoutheastCorner.Longitude.ToString(CultureInfo.InvariantCulture) + ","
                    + bbox.SoutheastCorner.Latitude.ToString(CultureInfo.InvariantCulture);
            }
            
            string url = BuildConfig.API_BASEURL + ApiConstants.END_POINT_NODES_SEATCH + "?"
                + ApiConstants.API_KEY_PARAM + "&"
                + searchParam + "&"
                + pageSizeParam + "&"
                + pageParam;

            if (bboxParam != null) {
                url += "&" + bboxParam;
            }

            return url;
        }


    }

    public class NodeTypeRequest : PagedRequest<NodeTypeResponse, NodeType> {
        protected override string GetUrl(int page) {
            string localeParam = "locale=" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            string pageParam = "page=" + page;
            string pageSizeParam = "page_size=" + PAGE_SIZE;
            string url = BuildConfig.API_BASEURL + ApiConstants.END_POINT_NODE_TYPES + "?"
                + ApiConstants.API_KEY_PARAM + "&"
                + pageSizeParam + "&"
                + pageParam + "&"
                + localeParam;
            return url;
        }
    }

}
