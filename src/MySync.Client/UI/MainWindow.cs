// MySync © 2016 Damian 'Erdroy' Korczowski


using System;
using System.Windows.Forms;

namespace MySync.Client.UI
{
    public partial class MainWindow : MetroFramework.Forms.MetroForm
    {
        internal static MainWindow Instance;

        public enum Screens
        {
            ProjectsMenu,
            ProjectView
        }

        private ProjectsMenu _projectsMenu;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            // initialize ProjectsMenu
            _projectsMenu = new ProjectsMenu
            {
                Dock = DockStyle.Fill,
                Visible = true
            };

            // add the control
            Controls.Add(_projectsMenu);
            
            // copyinfo label should be always visible ;)
            labelCopyinfo.BringToFront();
        }
    }
}