// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Windows.Forms;
using MySync.Core;

namespace MySync
{
    public partial class MainWindow : Form
    {

        public MainWindow()
        {
            Current = this;
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Controls.Add(ClientManager.Instance.Browser);
            MaximizedBounds = Screen.GetWorkingArea(this);
            
        }

        public static MainWindow Current { get; private set; }
    }
}