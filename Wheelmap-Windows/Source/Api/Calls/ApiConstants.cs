using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wheelmap_Windows.Model;

namespace Wheelmap_Windows.Api.Calls {

    public class ApiConstants {
        public const string END_POINT_NODES = "/api/nodes";
        public const string END_POINT_PHOTOS = "/api/nodes/{0}/photos";
        public const string END_POINT_CATEGORY = "/api/categories";
        public const string END_POINT_ASSETS = "/api/assets";
        public const string END_POINT_NODE_TYPES = "/api/node_types";
        public const string END_POINT_USER_AUTHENTICATE = "/api/users/authenticate";
        public const string END_POINT_USER_TERMS_ACCEPTED = "/api/users/accept_terms";
        public const string END_POINT_NODE_EDIT = "/api/nodes/{0}";
        public const string END_POINT_NODE_CREATE = "/api/nodes";
        public const string END_POINT_UPDATE_WHEELCHAIR_STATUS = "/api/nodes/{0}/update_wheelchair";

        // TODO replace with correct api
        public const string END_POINT_UPDATE_TOILET_STATUS = "/nodes/{0}/update_toilet.js";

        public const string WEB_LOGIN_LINK = "/users/auth/osm";
        public const string WM_REGISTER_LINK = "/en/oauth/register_osm";



        public static string API_KEY_PARAM {
            get {
                // use api token of the user if possible
                if (User.CurrentUser?.apiKey != null) {
                    return "api_key=" + User.CurrentUser?.apiKey;
                }
                return "api_key=" + BuildConfig.API_KEY;
            }
        }
    }
    
}
