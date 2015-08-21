using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Api.Model;
using Wheelmap_Windows.Extensions;
using Wheelmap_Windows.Utils.Extensions;

namespace Wheelmap_Windows.Api.Calls {

    public abstract class PagedRequest<T, K> where T : PagedResponse<K> {

        string TAG = "PagesRequest";

        public PagedRequest() {
            TAG = GetType().Name;
        }

        // For pagination, how many results to return per page. Default is 200. Max is 500.
        public const int PAGE_SIZE = 500;

        public async Task<List<K>> Query() {
            return await Task.Run(() => {
                List<K> items = new List<K>();

                // handle pages request
                int page = 0;
                long numPages = 1;
                while (page < numPages) {
                    var resp = QueryPage(page + 1);
                    if (resp?.meta == null) {
                        break;
                    }
                    page = resp.meta.page;
                    numPages = resp.meta.numPages;
                    items.AddAll(resp.GetItems());
                    if (page == 1) {
                        Log.d(TAG, "Query: numPages = " + numPages + ": items: " + resp.meta.itemCountTotal);
                    }
                }

                return items;
            });
        }

        protected PagedResponse<K> QueryPage(int page) {

            string url = GetUrl(page);
            if (url == null) {
                return null;
            }
            Log.d(TAG, url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            if (response == null) {
                return null;
            }
            var json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return JsonConvert.DeserializeObject<T>(json);
        }

        /**
         * return the url to fetch for the next page
         */
        protected abstract string GetUrl(int page);

    }
}
