using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wheelmap.Api.Calls {

    public interface Request<T> {
        Task<T> Execute();

        /**
         * returns true if last execution ended with an error
         */
        bool WasError();
    }

    /**
     * wraps an request to try it multiple times
     */
    public class RetryRequest<T> : Request<T>{

        private Request<T> request;
        private int retries;
        private bool error;

        public RetryRequest(Request<T> request, int retries = 3) {
            this.retries = retries;
            this.request = request;
        }

        public async Task<T> Execute() {
            error = false;

            // make sure there is at least one request to get a valid result
            T result = await request.Execute();
            if (!request.WasError()) {
                return result;
            }
            for (int i = 1; i < retries; i++) {
                result = await request.Execute();
                if (!request.WasError()) {
                    return result;
                }
            }
            error = true;
            return result;
        }

        public bool WasError() {
            return error;
        }
    }
}
