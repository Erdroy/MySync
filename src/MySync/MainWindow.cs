// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Windows.Forms;
using MySync.Core;
using MySync.Projects;

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
            
            ClientManager.Instance.Browser.FrameLoadEnd += delegate
            {
                // load all projects
                ProjectManager.Instance.LoadAll();
            };
        }

        public static MainWindow Current { get; private set; }
    }
}