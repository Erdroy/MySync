// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MySync.Server.Core
{
    /// <summary>
    /// RequestProcessor class.
    /// Handles and processes http requests.
    /// </summary>
    public class RequestProcessor : IDisposable
    {
        private readonly Dictionary<string, Action<HttpListenerRequest, HttpListenerResponse>> _requestHandles = 
            new Dictionary<string, Action<HttpListenerRequest, HttpListenerResponse>>();

        internal RequestProcessor()
        {
            _requestHandles.Add("/check", CheckHandler);
        }

        public void Process(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            
            var url = request.Url.LocalPath;
            
            // set response as 'OK'/succeeded
            response.StatusCode = 200;
            response.StatusDescription = "OK";
            response.AddHeader("Content-Type", "application/json");

            // try to find handler
            Action<HttpListenerRequest, HttpListenerResponse> handler;
            if (!_requestHandles.TryGetValue(url, out handler))
            {
                var notfound = Encoding.UTF8.GetBytes("{\"error\":\"NOT FOUND\"}");
                response.OutputStream.Write(notfound, 0, notfound.Length);
                return;
            }

            // handle
            handler.Invoke(request, response);
        }

        public void Dispose()
        {

        }

        private void CheckHandler(HttpListenerRequest request, HttpListenerResponse response)
        {
            var buffer = Encoding.UTF8.GetBytes("{\"status\":0}");
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }
    }
}
