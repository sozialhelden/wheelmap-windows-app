using System.Globalization;
using Wheelmap.Api.Model;
using Wheelmap.Model;

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
