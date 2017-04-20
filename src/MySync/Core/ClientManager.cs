// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Drawing;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;
using MySync.Callbacks;

namespace MySync.Core
{
    /// <summary>
    /// The client manager class.
    /// </summary>
    public class ClientManager : IDisposable
    {
        /// <summary>
        /// Runs this instance of client manager.
        /// </summary>
        public void Run()
        {
            // enable high dpi cef rendering support
            Cef.EnableHighDPISupport();

            var settings = new CefSettings
            {
                RemoteDebuggingPort = 8088,
                CachePath = "cache",
                MultiThreadedMessageLoop = true,
                ExternalMessagePump = false,
                FocusedNodeChangedEnabled = true
            };

            // disable proxy server
            settings.CefCommandLineArgs.Add("no-proxy-server", "1");

            if (!Cef.Initialize(settings))
            {
                throw new Exception("Unable to Initialize Cef");
            }

            var uriBuilder = new UriBuilder(Environment.CurrentDirectory.Replace('\\', '/') + "/ui/index.html");
            var uri = uriBuilder.Uri.ToString();
            Browser = new ChromiumWebBrowser(uri)
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(35, 35, 35),
                MenuHandler = new Program.CustomContextHandler()
            };

            Browser.RegisterJsObject("windowEvents", new WindowCallbacks(), BindingOptions.DefaultBinder);
            Browser.RegisterJsObject("projectEvents", new ProjectCallbacks(), BindingOptions.DefaultBinder);

        }

        /// <summary>
        /// Disposes the current instance of client manager.
        /// </summary>
        public void Dispose()
        {
            _instance = null;
            Browser.Dispose();
            Cef.Shutdown();
        }

        /// <summary>
        /// The default browser used to render UI.
        /// </summary>
        public ChromiumWebBrowser Browser { get; private set; }
        
        /// <summary>
        /// The current instance of client manager.
        /// </summary>
        public static ClientManager Instance => _instance ?? (_instance = new ClientManager());
        private static ClientManager _instance;
    }
}
