using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Model;
using System.Net.Http;
using System.Globalization;
using Wheelmap_Windows.Api.Model;
using Newtonsoft.Json;

/**
 * http://wheelmap.org/ja/api/docs/resources/nodes
 */
namespace Wheelmap_Windows.Api.Calls {

    class NodeEditHelper {

        public static IDictionary<string, string> NodeToParams(Node n) {
            var dic = new Dictionary<string, string> {
                // fill required fields
                { "name",  n.name },
                { "type",  n.nodeType.id.ToString(CultureInfo.InvariantCulture) },
                { "lat", n.lat.ToString(CultureInfo.InvariantCulture) },
                { "lon", n.lon.ToString(CultureInfo.InvariantCulture) }
            };

            if (n.wheelchairStatus != null) {
                dic.Add("wheelchair", n.wheelchairStatus);
            }

            if (n.wheelchairDescription != null) {
                dic.Add("wheelchair_description", n.wheelchairDescription);
            }
            
            // TODO how to change wheelchairWCStatus

            if (n.street != null) {
                dic.Add("street", n.street);
            }

            if (n.housenumber != null) {
                dic.Add("housenumber", n.housenumber);
            }

            if (n.city != null) {
                dic.Add("city", n.city);
            }

            if (n.postcode != null) {
                dic.Add("postcode", n.postcode);
            }

            if (n.website != null) {
                dic.Add("website", n.website);
            }

            if (n.phone != null) {
                dic.Add("phone", n.phone);
            }

            return dic;
        }

    }

    public class NodeEditRequest {

        Node node;

        public NodeEditRequest(Node  node) {
            this.node = node;
        }

        public async Task<bool> Execute() {

            using (var client = new HttpClient()) {
                string url = GetUrl();
                Log.d(this, "Post to: " + url);

                var values = NodeEditHelper.NodeToParams(node);
                var content = new FormUrlEncodedContent(values);
                HttpResponseMessage responseMessage;
                if (node.id <= 0) {
                    responseMessage = await client.PostAsync(url, content);
                } else {
                    responseMessage = await client.PutAsync(url, content);
                }
                var responseString = await responseMessage.Content.ReadAsStringAsync();
                var editResponse = JsonConvert.DeserializeObject<NodeEditResponse>(responseString);
                return editResponse.IsOk;
            }

        }

        protected string GetUrl() {
            string url = BuildConfig.API_BASEURL + String.Format(ApiConstants.END_POINT_NODE_EDIT, node.id) + "?"
                + ApiConstants.API_KEY_PARAM;
            return url;
        }

    }
}
