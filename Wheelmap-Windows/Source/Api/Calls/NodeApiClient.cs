﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Api.Model;
using Wheelmap_Windows.Model;
using Wheelmap_Windows.Utils.Extensions;
using Windows.Devices.Geolocation;
using Windows.System.Threading;

/**
 * contains all methods to query Node from the Wheelmap Api
 * @see http://wheelmap.org/api/docs/resources/nodes 
 */
namespace Wheelmap_Windows.Api.Calls {
    
    public class NodesRequest : PagedRequest<NodesResponse, Node> {

        GeoboundingBox bbox;

        public NodesRequest(GeoboundingBox bbox) {
            this.bbox = bbox;
        }

        public override PagedResponse<Node> QueryPage(int page) {
            string pageParam = "page=" + page;
            string pageSizeParam = "page_size=" + PAGE_SIZE;
            string bboxParam = "bbox="
                + bbox.NorthwestCorner.Longitude.ToString(CultureInfo.InvariantCulture) + ","
                + bbox.NorthwestCorner.Latitude.ToString(CultureInfo.InvariantCulture) + ","
                + bbox.SoutheastCorner.Longitude.ToString(CultureInfo.InvariantCulture) + ","
                + bbox.SoutheastCorner.Latitude.ToString(CultureInfo.InvariantCulture);

            string url = BuildConfig.API_BASEURL + ApiConstants.END_POINT_NODES + "?"
                + BuildConfig.API_KEY_PARAM + "&"
                + bboxParam + "&"
                + pageSizeParam + "&"
                + pageParam;

            Log.d(this, "GetNodes: " + url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            if (response == null) {
                return null;
            }
            var json = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return JsonConvert.DeserializeObject<NodesResponse>(json);

        }
    }
}
