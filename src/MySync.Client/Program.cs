// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System;
using System.Windows.Forms;
using MySync.Client.UI;

namespace MySync.Client
{
    internal static class Program
    {
        [MTAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWindow());
        }
    }
}
