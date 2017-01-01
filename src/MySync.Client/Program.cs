// MySync © 2016-2017 Damian 'Erdroy' Korczowski


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

            ClientSettings.Load();
            ProjectsManager.Instance.Initialize();

            Application.Run(new MainWindow());
        }
    }
}
