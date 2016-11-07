// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System;
using System.Windows.Forms;
using MySync.Client.Core.Projects;
using MySync.Client.UI;
using MySync.Client.Utilities;

namespace MySync.Client
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // initialize project manager
            InfoBox.ShowMessage("Connecting to server");
            {
                ClientSettings.Load();
                ProjectsManager.Instance.Initialize();
                InfoBox.HideMessage();
            }

            Application.Run(new MainWindow());
        }
    }
}
