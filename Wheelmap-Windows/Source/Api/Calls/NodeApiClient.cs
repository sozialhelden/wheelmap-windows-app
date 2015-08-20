using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Api.Model;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.Utils.Extensions;
using Windows.Devices.Geolocation;

namespace Wheelmap_Windows.Api.Calls {

    /**
     * contains all methods to query Node from the Wheelmap Api
     * @see http://wheelmap.org/api/docs/resources/nodes 
     */
    public class NodeApiClient {

        const string TAG = nameof(NodeApiClient);

        const string END_POINT_NODES = "/api/nodes";

        // For pagination, how many results to return per page. Default is 200. Max is 500.
        const int PAGE_SIZE = 500;

        public static Node[] GetNodes(GeoboundingBox bbox) {
            
            string pageSizeParam = "page_size=" + PAGE_SIZE;
            string bboxParam = "bbox="
                + bbox.NorthwestCorner.Longitude.ToString(CultureInfo.InvariantCulture) + ","
                + bbox.NorthwestCorner.Latitude.ToString(CultureInfo.InvariantCulture) + ","
                + bbox.SoutheastCorner.Longitude.ToString(CultureInfo.InvariantCulture) + ","
                + bbox.SoutheastCorner.Latitude.ToString(CultureInfo.InvariantCulture);

            string url = BuildConfig.API_BASEURL + END_POINT_NODES + "?"
                + BuildConfig.API_KEY_PARAM + "&"
                + bboxParam + "&"
                + pageSizeParam;

            Log.d(TAG, "GetNodes: "+url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            var json = new StreamReader(response.GetResponseStream()).ReadToEnd();


            var result = JsonConvert.DeserializeObject<NodesResponse>(json);

            Log.d(TAG, "Nodes queries: " + result.nodes.Length);
            return result.nodes;

        }

    }

}
