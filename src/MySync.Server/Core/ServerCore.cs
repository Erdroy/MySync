// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.IO;
using System.Net;
using MySync.Client.Core;
using Newtonsoft.Json;

namespace MySync.Server.Core
{
    /// <summary>
    /// ServerCore class.
    /// Handles client connections etc.
    /// </summary>
    public class ServerCore : IDisposable
    {
        private HttpListener _httpListener;
        private bool _isDisposed;

        private RequestProcessor _processor;

        /// <summary>
        /// Default ServerCore class constructor.
        /// </summary>
        internal ServerCore()
        {
            _isDisposed = false;
        }

        /// <summary>
        /// Run the ServerCore.
        /// </summary>
        internal void Run()
        {
            _isDisposed = false;

            // load projects settings
            Settings = JsonConvert.DeserializeObject<ProjectsSettings>(File.ReadAllText("serversettings.json"));

            // initialize request processor
            _processor = new RequestProcessor();

            // initialize all handlers
            LoadHandlers();

            // initialize http listener
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("http://127.0.0.1:8080/");  // TODO: https!
            _httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

            // start listening
            _httpListener.Start();

            Console.WriteLine("MySync (c) 2016-2017 Damian 'Erdroy' Korczowski github.com/Erdroy");
            Console.WriteLine("MySync server is running on 8080 port");

            // listener loop
            while (!_isDisposed)
            {
                var result = _httpListener.BeginGetContext(ProcessCallback, _httpListener);
                result.AsyncWaitHandle.WaitOne();
            }
        }

        /// <summary>
        /// Dispose the server core.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
        }
        
        // private
        private void ProcessCallback(IAsyncResult result)
        {
            var context = _httpListener.EndGetContext(result);

            // handle request
            _processor.Process(context);

            context.Response.Close();
        }

        // private
        private void LoadHandlers()
        {
            _processor.AddHandler("/authorize", RequestHandlers.Authorize.Process);
            _processor.AddHandler("/pull", RequestHandlers.VersionControl.Pull);

            _processor.AddDownloader("/push", RequestHandlers.VersionControl.Push);
        }

        public static ProjectsSettings Settings { get; private set; }
    }
}
