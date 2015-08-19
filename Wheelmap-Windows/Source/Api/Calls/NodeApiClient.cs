using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Api.Model;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.Utils.Extensions;

namespace Wheelmap_Windows.Api.Calls {

    /**
     * contains all methods to query Node from the Wheelmap Api
     * @see http://wheelmap.org/api/docs/resources/nodes 
     */
    public class NodeApiClient {

        const string END_POINT_NODES = "/api/nodes";

        public static Node[] GetNodes() {

            //TODO bounding box

            string url = BuildConfig.API_BASEURL + END_POINT_NODES + "?" + BuildConfig.API_KEY_PARAM;
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            var json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            var result = JsonConvert.DeserializeObject<NodesRequest>(json);

            return result.nodes;
        }

    }

}
