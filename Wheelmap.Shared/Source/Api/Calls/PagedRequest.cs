using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Wheelmap.Api.Model;
using Wheelmap.Extensions;
using Wheelmap.Utils.Extensions;

namespace Wheelmap.Api.Calls {

    /// <summary>
    /// the most api calls are paged requests
    /// http://wheelmap.org/de/api/docs
    /// This base class wraps the most handling about using this mechanism
    /// </summary>
    public abstract class PagedRequest<T, K> : Request<List<K>> where T : PagedResponse<K>{

        private bool error = false;
        protected bool SinglePage = false;
        private TaskFactory mTaskFactory;

        /// <summary>
        /// to make sure that we cannot run into an infinite loop which query pages
        /// </summary>
        protected int MaxPages = 5;

        public PagedRequest(bool singlePage = false) : this(null, singlePage) {}

        public PagedRequest(TaskFactory factory, bool singlePage = false) {
            mTaskFactory = factory;
            SinglePage = singlePage;
        }

        // For pagination, how many results to return per page. Default is 200. Max is 500.
        public const int PAGE_SIZE = 500;

        public virtual async Task<List<K>> Execute() {
            error = false;
            var start = DateTime.Now;

            Task<List<K>> task;
            if (mTaskFactory != null) {
                task = mTaskFactory.StartNew(QueryPages);
            } else {
                task = Task.Run(() => QueryPages());
            }
            var result = await task;

            Log.i("QueryTime: " + (DateTime.Now - start));
            start = DateTime.Now;

            result = await prepareData(result);

            Log.i("DataPrepareTime: " + (DateTime.Now - start));
            error = result == null;
            return result;
        }

        /// <summary>
        /// checks if last request had an error
        /// </summary>
        public bool WasError() {
            return error;
        }

        /// <summary>
        /// collects all available pages and merges their result
        /// </summary>
        /// <returns>
        /// null if an error happens
        /// </returns>
        protected virtual List<K> QueryPages() {
            List<K> items = new List<K>();

            int iteration = 0;
            // handle pages request
            int page = 0;
            long numPages = 1;
            while (page < numPages && iteration < MaxPages) {
                try {
                    var resp = QueryPage(page + 1);
                    if (resp?.meta == null) {
                        break;
                    }
                    page = resp.meta.page;
                    numPages = resp.meta.numPages;
                    items.AddAll(resp.GetItems());

                    if (SinglePage) {
                        break;
                    }

                    if (page == 1) {
                        Log.d("Query: numPages = " + numPages + ": items: " + resp.meta.itemCountTotal);
                    }
                } catch (Exception e){
                    Log.e(e);
                    return null;
                }
                iteration++;
            }
            
            return items;
        }

        protected PagedResponse<K> QueryPage(int page) {

            string url = GetUrl(page);
            if (url == null) {
                return null;
            }
            Log.d(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            if (response == null) {
                return null;
            }
            var json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        ///  return the url to fetch for the next page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        protected abstract string GetUrl(int page);

        /// <summary>
        /// prepare data if needed
        /// </summary>
        protected virtual Task<List<K>> prepareData(List<K> items) {
            return Task.FromResult(items);
        }
        
    }

}
