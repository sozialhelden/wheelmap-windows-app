﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Wheelmap.Utils.Extensions {
    public static class WebRequestExtensions {

        public static WebResponse GetResponse(this WebRequest request) {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            IAsyncResult asyncResult = request.BeginGetResponse(r => autoResetEvent.Set(), null);

            // Wait until the call is finished
            autoResetEvent.WaitOne();
            try { 
                return request.EndGetResponse(asyncResult);
            }catch(WebException e) {
                Log.e(request.RequestUri, e.StackTrace);
                return null;
            }
        }

        public static Stream GetRequestStream(this WebRequest request) {
            AutoResetEvent autoResetEvent = new AutoResetEvent(false);

            IAsyncResult asyncResult = request.BeginGetRequestStream(r => autoResetEvent.Set(), null);

            // Wait until the call is finished
            autoResetEvent.WaitOne();

            return request.EndGetRequestStream(asyncResult);
        }

    }
}