using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wheelmap.Model;
using System.Net.Http;
using System.Globalization;
using Wheelmap.Api.Model;
using Newtonsoft.Json;
using Wheelmap.Extensions;

/**
 * http://wheelmap.org/ja/api/docs/resources/nodes
 */
namespace Wheelmap.Api.Calls {

    public class NodeEditRequest : Request<NodeEditResponse> {

        private bool error;
        Node node;

        public NodeEditRequest(Node  node) {
            this.node = node;
        }

        public async Task<NodeEditResponse> Execute() {

            if (node.DirtyState == DirtyState.CLEAN) {
                error = false;
                return new NodeEditResponse(ok: true);
            }

            error = false;
            try {
                return await execute();
            }
            catch {
                error = true;
                return new NodeEditResponse {
                    message = "ok",
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
                if (editResponse.IsOk) {
                    node.DirtyState = DirtyState.CLEAN;
                    Nodes.Save(node);
                }
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

        public bool WasError() {
            return error;
        }
    }
    
    public class NodeStateEditRequest : Request<NodeEditResponse> {

        private bool error;
        protected Node node;

        public NodeStateEditRequest(Node node) {
            this.node = node;
        }

        public async Task<NodeEditResponse> Execute() {

            if (node.DirtyState == DirtyState.CLEAN) {
                error = false;
                return new NodeEditResponse(ok: true);
            }

            error = false;
            try {
                var result = await execute();
                Log.d(this, $"{node}: success");
                return result;
            }
            catch {
                error = true;
                Log.d(this, $"{node}: error");
                return new NodeEditResponse {
                    message = "error",
                    error = new Dictionary<string, string[]> {
                        { "name",  new string[] {"UNKNOWN_ERROR".t()} }
                    }
                };
            }
        }

        public bool WasError() {
            return error;
        }

        protected async virtual Task<NodeEditResponse> execute() {
            using (var client = new HttpClient()) {
                string url = GetUrl();
                HttpResponseMessage responseMessage;
                responseMessage = await client.PutAsync(url, null);
                var responseString = await responseMessage.Content.ReadAsStringAsync();
                var editResponse = JsonConvert.DeserializeObject<NodeEditResponse>(responseString);
                if (editResponse.IsOk) {
                    node.DirtyState = DirtyState.CLEAN;
                    Nodes.Save(node);
                }
                return editResponse;
            }
        }

        protected virtual string GetUrl() {
            var endPoint = String.Format(ApiConstants.END_POINT_UPDATE_WHEELCHAIR_STATUS, node.wm_id);
            string localeParam = "locale=" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            var paramStatus = "wheelchair=" + node.wheelchairStatus;

            string url = BuildConfig.API_BASEURL + endPoint + "?"
                + ApiConstants.API_KEY_PARAM + "&"
                + paramStatus + "&"
                + localeParam;
            return url;
        }

    }

    // small hack until real api is present
    // http://staging.wheelmap.org/nodes/433727969/update_toilet.js?api_key=XXXX&toilet=no
    public class NodeEditToiletStatusRequest : NodeStateEditRequest {

        public NodeEditToiletStatusRequest(Node node) : base(node) {
        }

        protected async override Task<NodeEditResponse> execute() {
            using (var client = new HttpClient()) {
                string url = GetUrl();
                HttpResponseMessage responseMessage;
                responseMessage = await client.PutAsync(url, null);
                var responseString = await responseMessage.Content.ReadAsStringAsync();
                if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK) {
                    node.DirtyState = DirtyState.CLEAN;
                    Nodes.Save(node);
                    return new NodeEditResponse(ok: true);
                }
                return new NodeEditResponse(ok: false);
            }
        }

        protected override string GetUrl() {
            var endPoint = String.Format(ApiConstants.END_POINT_UPDATE_TOILET_STATUS, node.wm_id);
            var paramStatus = "toilet=" + node.wheelchairToiletStatus;

            string url = BuildConfig.API_BASEURL + endPoint + "?"
                + ApiConstants.API_KEY_PARAM + "&"
                + paramStatus;
            return url;
        }
    }
}
