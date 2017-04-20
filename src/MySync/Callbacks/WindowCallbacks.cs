// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Drawing;
using System.Windows.Forms;

namespace MySync.Callbacks
{
    /// <summary>
    /// The window callbacks class.
    /// </summary>
    public class WindowCallbacks
    {
        private bool _drag;
        private Point _dragStart;
        private Point _dragStartLocation;

        /// <summary>
        /// JS Callback
        /// </summary>
        public void CloseWindow()
        {
            Application.Exit();
        }

        /// <summary>
        /// JS Callback
        /// </summary>
        public void MaximizeWindow()
        {
            MainWindow.Current.BeginInvoke((Action) (() => // invoke at main thread
            {
                MainWindow.Current.WindowState = MainWindow.Current.WindowState != FormWindowState.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
            }));
        }

        /// <summary>
        /// JS Callback
        /// </summary>
        public void MinimizeWindow()
        {
            MainWindow.Current.BeginInvoke((Action)(() => // invoke at main thread
            {
                MainWindow.Current.WindowState = FormWindowState.Minimized;
            }));
        }

        /// <summary>
        /// JS Callback
        /// </summary>
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