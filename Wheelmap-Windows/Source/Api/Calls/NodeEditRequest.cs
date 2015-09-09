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
using Wheelmap_Windows.Extensions;

/**
 * http://wheelmap.org/ja/api/docs/resources/nodes
 */
namespace Wheelmap_Windows.Api.Calls {
    
    public class NodeEditRequest {

        Node node;

        public NodeEditRequest(Node  node) {
            this.node = node;
        }

        public async Task<NodeEditResponse> Execute() {
            try {
                return await execute();
            }
            catch {
                return new NodeEditResponse {
                    message = "error",
                    error = new Dictionary<string, string[]> {
                        { "name",  new string[] {"UNKNOWN_ERROR".t()} }
                    }
                };
            }
        }
        
        private async Task<NodeEditResponse> execute() {
            using (var client = new HttpClient()) {
                string url = GetUrl();
                var values = NodeToParams(node);
                var content = new FormUrlEncodedContent(values);
                HttpResponseMessage responseMessage;
                if (node.wm_id <= 0) {
                    Log.d(this, "Create Node " + url);
                    responseMessage = await client.PostAsync(url, content);
                } else {
                    Log.d(this, "Update Node: " + url);
                    responseMessage = await client.PutAsync(url, content);
                }
                var responseString = await responseMessage.Content.ReadAsStringAsync();
                var editResponse = JsonConvert.DeserializeObject<NodeEditResponse>(responseString);
                return editResponse;
            }
        }

        protected string GetUrl() {
            var endPoint = node.wm_id > 0 ? String.Format(ApiConstants.END_POINT_NODE_EDIT, node.wm_id) : ApiConstants.END_POINT_NODE_CREATE;
            string localeParam = "locale=" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            string url = BuildConfig.API_BASEURL + endPoint + "?"
                + ApiConstants.API_KEY_PARAM + "&"
                + localeParam;
            return url;
        }

        public static IDictionary<string, string> NodeToParams(Node n) {
            var dic = new Dictionary<string, string> {
                // fill required fields
                { "name",  n.name },
                { "type",  n.nodeType.identifier },
                { "lat", n.lat.ToString(CultureInfo.InvariantCulture) },
                { "lon", n.lon.ToString(CultureInfo.InvariantCulture) }
            };

            if (n.wheelchairStatus != null) {
                dic.Add("wheelchair", n.wheelchairStatus);
            }

            if (n.wheelchairDescription != null) {
                dic.Add("wheelchair_description", n.wheelchairDescription);
            }

            if (n.wheelchairToiletStatus != null) {
                dic.Add("wheelchair_toilet", n.wheelchairToiletStatus);
            }

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
}
