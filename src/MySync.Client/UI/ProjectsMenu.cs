// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System.Windows.Forms;
using MetroFramework.Controls;
using MySync.Client.Core.Projects;
using MySync.Client.Utilities;

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
            foreach (var projectToOpen in ClientSettings.Instance.OpenedProjects)
            {
                CreateProjectView(ProjectsManager.Instance.OpenProject(projectToOpen.Name, projectToOpen.LocalDir));
            }
        }

        private void labelNewProject_Click(object sender, System.EventArgs e)
        {
            // TODO: Show project creation window
        }

        public void CreateProjectView(Project project)
        {
            var tab = new MetroTabPage
            {
                Text = project.Name
            };

            var view = new ProjectView
            {
                Dock = DockStyle.Fill
            };

            tab.Controls.Add(view);
            tabs.TabPages.Add(tab);

            view.LoadProject(project);
        }
    }
}