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
    public class RequestProcessor
    {
        // private
        private readonly Dictionary<string, Action<string, HttpListenerResponse>> _requestHandles = 
            new Dictionary<string, Action<string, HttpListenerResponse>>();

        private readonly Dictionary<string, Action<HttpListenerRequest, HttpListenerResponse>> _downloaders =
            new Dictionary<string, Action<HttpListenerRequest, HttpListenerResponse>>();

        /// <summary>
        /// Default RequestProcessor class constructor.
        /// </summary>
        internal RequestProcessor()
        {
            _requestHandles.Add("/check", CheckHandler);
        }

        /// <summary>
        /// Adds request handler.
        /// </summary>
        /// <param name="path">The path, eg.: /help/getDocs </param>
        /// <param name="action">The target handler-method.</param>
        public void AddHandler(string path, Action<string, HttpListenerResponse> action)
        {
            // add
            _requestHandles.Add(path, action);
        }

        /// <summary>
        /// Adds server side downloader.
        /// </summary>
        /// <param name="path">The path, eg.: /help/getDocs </param>
        /// <param name="action">The target downloader-method.</param>
        public void AddDownloader(string path, Action<HttpListenerRequest, HttpListenerResponse> action)
        {
            // add
            _downloaders.Add(path, action);
        }

        /// <summary>
        /// Process context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Process(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;
            
            var url = request.Url.LocalPath;
            
            // set response as 'OK'/succeeded
            response.StatusCode = 200;
            response.StatusDescription = "OK";
            response.AddHeader("Content-Type", "application/json");

            // try to handle by downloader
            if (_downloaders.ContainsKey(url))
            {
                _downloaders[url](request, response);
                return;
            }

            // try to find handler
            Action<string, HttpListenerResponse> handler;
            if (!_requestHandles.TryGetValue(url, out handler))
            {
                var notfound = Encoding.UTF8.GetBytes("{\"error\":\"NOT FOUND\"}");
                response.OutputStream.Write(notfound, 0, notfound.Length);
                return;
            }

            // handle
            using (var body = request.InputStream)
            {
                using (var reader = new System.IO.StreamReader(body, request.ContentEncoding))
                {
                    handler.Invoke(reader.ReadToEnd(), response);
                }
            }
        }

        // private
        private void CheckHandler(string body, HttpListenerResponse response)
        {
            var buffer = Encoding.UTF8.GetBytes("{\"status\":0}");
            response.OutputStream.Write(buffer, 0, buffer.Length);
        }
    }
}
