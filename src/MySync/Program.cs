using System;
using System.Security.Policy;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace MySync
{
    /// <summary>
    /// Program class.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Cef.EnableHighDPISupport();

            var settings = new CefSettings
            {
                RemoteDebuggingPort = 8088,
                CachePath = "cache",
                MultiThreadedMessageLoop = true,
                ExternalMessagePump = false,
                FocusedNodeChangedEnabled = true
            };

            settings.CefCommandLineArgs.Add("no-proxy-server", "1");

            if (!Cef.Initialize(settings))
            {
                throw new Exception("Unable to Initialize Cef");
            }

            var uriBuilder = new UriBuilder(Environment.CurrentDirectory.Replace('\\', '/') + "/ui/index.html");
            var uri = uriBuilder.Uri.ToString();
            Browser = new ChromiumWebBrowser(uri)
            {
                Dock = DockStyle.Fill
            };

            Application.Run(new MainWindow());
        }

        /// <summary>
        /// The default browser used to render UI.
        /// </summary>
        public static ChromiumWebBrowser Browser { get; private set; }
    }
}
