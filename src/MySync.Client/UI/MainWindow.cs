// MySync © 2016 Damian 'Erdroy' Korczowski


using System;
using System.Windows.Forms;
using MySync.Client.Core;

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
        private Timer _dispatcher;

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

            // run dispatch timer
            _dispatcher = new Timer
            {
                Interval = 250 // every 1/4 of a second.
            };
            
            _dispatcher.Tick += delegate
            {
                if (Lock)
                    return;

                // dispatch events
                TaskManager.DispatchEvents();
            };
            _dispatcher.Start();
        }

        protected override void OnClosed(EventArgs e)
        {
            // dispose all
            _dispatcher.Dispose();
        }

        public static bool Lock { get; set; }
    }
}