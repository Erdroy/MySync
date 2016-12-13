// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System.Windows.Forms;
using MetroFramework.Controls;

namespace MySync.Client.UI
{
    public partial class ProjectsMenu : MetroUserControl
    {
        public ProjectsMenu()
        {
            InitializeComponent();
        }

        private void ProjectsMenu_Load(object sender, System.EventArgs e)
        {
            // TODO: Load all projects
        }

        private void labelNewProject_Click(object sender, System.EventArgs e)
        {
            var tab = new MetroTabPage
            {
                Text = @"New Project"
            };

            // TODO: Show project creation window
            var view = new ProjectView
            {
                Dock = DockStyle.Fill
            };

            tab.Controls.Add(view);

            tabs.TabPages.Add(tab);
        }
    }
}