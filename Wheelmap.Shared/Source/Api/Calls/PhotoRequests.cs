using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Wheelmap.Api.Model;
using Wheelmap.Extensions;
using Wheelmap.Model;
using Windows.Storage;

namespace Wheelmap.Api.Calls {

    public class PhotosRequest : PagedRequest<PhotosResponse, Photo> {

        Node node;
        public PhotosRequest(Node n) {
            this.node = n;
        }

        protected override string GetUrl(int page) {
            string pageParam = "page=" + page;
            string pageSizeParam = "page_size=" + PAGE_SIZE;
            string url = BuildConfig.API_BASEURL + String.Format(ApiConstants.END_POINT_PHOTOS, node.wm_id) + "?"
                + ApiConstants.API_KEY_PARAM + "&"
                + pageSizeParam + "&"
                + pageParam;
            return url;
        }

    }

    public class PhotoUploadTask : Request<NodeEditResponse> {

        protected Node node;
        protected StorageFile file;
        protected bool error;

        public PhotoUploadTask(Node node, StorageFile file) {
            this.node = node;
            this.file = file;
        }

        public async Task<NodeEditResponse> Execute() {
            error = false;
            try {
                var result = await execute();
                error = !result.IsOk;
                Log.d($"{node}: success: " + !error);
                return result;
            } catch {
                error = true;
                Log.d($"{node}: error");
                return new NodeEditResponse {
                    message = "error",
                    error = new Dictionary<string, string[]> {
                        { "name",  new string[] {"UNKNOWN_ERROR".t()} }
                    }
                };
            }
        }
        
        protected async virtual Task<NodeEditResponse> execute() {
            using (var client = new HttpClient()) {
                var content = new MultipartFormDataContent();
                var randomAccess = await file.OpenReadAsync();
                
                content.Add(new StreamContent(randomAccess.AsStreamForRead()), "photo", file.Name);
                string url = GetUrl();
                HttpResponseMessage responseMessage;
                responseMessage = await client.PostAsync(url, content);
                var responseString = await responseMessage.Content.ReadAsStringAsync();
                Log.d(responseString);
                var editResponse = JsonConvert.DeserializeObject<NodeEditResponse>(responseString);
                return editResponse;
            }
        }

        protected string GetUrl() {
            string url = BuildConfig.API_BASEURL + String.Format(ApiConstants.END_POINT_PHOTOS, node.wm_id) + "?"
                + ApiConstants.API_KEY_PARAM;
            return url;
        }

        public bool WasError() {
            return error;
        }
    }
}
