using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap_Windows.Api.Model {

    /**
     * meta data for paged requests
     */
    class Meta {

        long page;

        [JsonProperty(PropertyName = "num_pages")]
        long numPages;

        [JsonProperty(PropertyName = "item_count")]
        long itemCount;

        [JsonProperty(PropertyName = "item_count_total")]
        long itemCountTotal;

    }
}
