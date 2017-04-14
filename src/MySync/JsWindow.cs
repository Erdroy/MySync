// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Windows.Forms;

namespace MySync
{
    public class JsWindow
    {
        public void CloseWindow()
        {
            Application.Exit();
        }

        public void MaximizeWindow()
        {
            MainWindow.Current.BeginInvoke((Action) (() => // invoke at main thread
            {
                MainWindow.Current.WindowState = MainWindow.Current.WindowState != FormWindowState.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
            }));
        }

        public void MinimizeWindow()
        {
            MainWindow.Current.BeginInvoke((Action)(() => // invoke at main thread
            {
                MainWindow.Current.WindowState = FormWindowState.Minimized;
            }));
        }
    }
}