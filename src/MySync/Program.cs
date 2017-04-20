// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Windows.Forms;
using CefSharp;
using MySync.Core;

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
            // run client manager
            ClientManager.Instance.Run();

            // create mysync main-window
            Application.Run(new MainWindow());
        }
    }
}
