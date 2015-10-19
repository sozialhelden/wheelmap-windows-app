using Newtonsoft.Json;

namespace Wheelmap.Api.Model {

    /**
     * meta data for paged requests
     */
    public class Meta {

        public int page;

        [JsonProperty(PropertyName = "num_pages")]
        public long numPages;

        [JsonProperty(PropertyName = "item_count")]
        public long itemCount;

        [JsonProperty(PropertyName = "item_count_total")]
        public long itemCountTotal;

    }

    public class Conditions {
        public int page;

        [JsonProperty(PropertyName = "per_page")]
        public int perPage;

        public string locale;
        public string format;
    }
}
