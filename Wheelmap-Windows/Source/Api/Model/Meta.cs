﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap_Windows.Api.Model {

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
}
