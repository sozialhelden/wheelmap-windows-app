using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Api.Model;
using Wheelmap_Windows.Extensions;

namespace Wheelmap_Windows.Api.Calls {
    
    public abstract class PagedRequest<T,K> where T : PagedResponse<K> {

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
                        Log.d(TAG, "Query: numPages = " + numPages+ ": items: "+resp.meta.itemCountTotal);
                    }
                }

                return items;
            });
        }

        public abstract PagedResponse<K> QueryPage(int page);

    }
}
