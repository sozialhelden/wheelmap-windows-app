using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Wheelmap.Api.Model;
using Wheelmap.Model;

namespace Wheelmap.Api.Calls {

    public class LoginRequest {

        private string username;
        private string password;

        public LoginRequest(string username, string password) {
            this.username = username;
            this.password = password;
        }

        public async Task<User> Query() {

            try {
                var url = GetUrl();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                WebResponse response = await request.GetResponseAsync();
                if (response == null) {
                    return null;
                }
                var json = new StreamReader(response.GetResponseStream()).ReadToEnd();
                var user = JsonConvert.DeserializeObject<UserAuthenticateResponse>(json).user;
                user.email = username;
                return user;

            } catch {
                // auth or network error
                return null;
            }

        }

        protected string GetUrl() {
            var paramEmail = "email=" + username;
            var paramPassword = "password=" + password;
            string url = BuildConfig.API_BASEURL + ApiConstants.END_POINT_USER_AUTHENTICATE + "?" +
                paramEmail + "&" +
                paramPassword;
            return url;
        }

    }

    /**
     * TODO if needed
     */
    public class UserTermsAcceptRequest {

        private bool termsAccepted;
        private bool privacyAccepted;

        public UserTermsAcceptRequest(bool termsAccepted, bool privacyAccepted) {
            this.termsAccepted = termsAccepted;
            this.privacyAccepted = privacyAccepted;
        }


        protected string GetUrl() {
            var paramTermsAccepted = "terms_accepted=" + termsAccepted;
            var paramPrivacyAccepted = "privacy_accepted=" + privacyAccepted;
            string url = BuildConfig.API_BASEURL + ApiConstants.END_POINT_USER_TERMS_ACCEPTED + "?" +
                paramTermsAccepted + "&" +
                paramPrivacyAccepted;
            return url;
        }

    }
}
