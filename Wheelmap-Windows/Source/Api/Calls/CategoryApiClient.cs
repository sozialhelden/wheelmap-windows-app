using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wheelmap.Api.Model;
using Wheelmap.Extensions;
using Wheelmap.Model;
using Wheelmap.Utils.Extensions;

namespace Wheelmap.Api.Calls {
    
    /**
     * http://wheelmap.org/api/docs/resources/categories
     */
    public class CategoryRequest : PagedRequest<CategoryResponse, Category> {

        protected override string GetUrl(int page) {
            string localeParam = "locale=" + CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            string pageParam = "page=" + page;
            string pageSizeParam = "page_size=" + PAGE_SIZE;
            string url = BuildConfig.API_BASEURL + ApiConstants.END_POINT_CATEGORY + "?"
                + ApiConstants.API_KEY_PARAM + "&"
                + pageSizeParam + "&"
                + page + "&"
                + localeParam;
            return url;
        }

    }
}
