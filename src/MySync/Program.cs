// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Drawing;
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
        // http://stackoverflow.com/questions/27802570/chromiumwebbrowser-disable-right-click-context-menu-c-sharp
        /// <summary>
        /// Disables context menu
        /// </summary>
        public class CustomContextHandler : IContextMenuHandler
        {
            public void OnBeforeContextMenu(IWebBrowser browserControl, CefSharp.IBrowser browser, IFrame frame, IContextMenuParams parameters,
                IMenuModel model)
            {
                model.Clear();
            }

            public bool OnContextMenuCommand(IWebBrowser browserControl, CefSharp.IBrowser browser, IFrame frame, IContextMenuParams parameters,
                CefMenuCommand commandId, CefEventFlags eventFlags)
            {
                return false;
            }

            public void OnContextMenuDismissed(IWebBrowser browserControl, CefSharp.IBrowser browser, IFrame frame)
            {
            }

            public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters,
                IMenuModel model, IRunContextMenuCallback callback)
            {
                return false;
            }
        }
        
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
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(35, 35, 35),
                MenuHandler = new CustomContextHandler()
            };

            Browser.RegisterJsObject("jswindow", new JsWindow(), BindingOptions.DefaultBinder);
            
            Application.Run(new MainWindow());
        }

        /// <summary>
        /// The default browser used to render UI.
        /// </summary>
        public static ChromiumWebBrowser Browser { get; private set; }
    }
}
