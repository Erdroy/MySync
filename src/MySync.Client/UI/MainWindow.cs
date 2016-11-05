// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System;
using System.Windows.Forms;

namespace MySync.Client.UI
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            var menu = new Menu();
            Controls.Add(menu);

            menu.Dock = DockStyle.Left;
            menu.Width = 250;
        }
    }
}
