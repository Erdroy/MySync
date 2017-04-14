// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Drawing;
using System.Windows.Forms;

namespace MySync
{
    public class JsWindow
    {
        private bool _drag;
        private Point _dragStart;
        private Point _dragStartLocation;

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

        public void MouseMove()
        {
            if (Control.MouseButtons == MouseButtons.Left && MainWindow.Current.WindowState != FormWindowState.Maximized)
            {
                if (_drag)
                {
                    var cpos = Cursor.Position;
                    var delta = new Point(cpos.X - _dragStart.X, cpos.Y - _dragStart.Y);

                    MainWindow.Current.BeginInvoke((Action)(() => // invoke at main thread
                    {
                        MainWindow.Current.Location = new Point(_dragStartLocation.X + delta.X, _dragStartLocation.Y + delta.Y);
                    }));
                }
                else
                {
                    _dragStartLocation = MainWindow.Current.Location;
                    _dragStart = Cursor.Position;
                    _drag = true;
                }
            }
            else
            {
                _drag = false;
            }
        }
    }
}