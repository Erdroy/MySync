// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.Windows.Forms;
using MetroFramework.Controls;
using MySync.Client.Core;
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
            // Load all projects
            Progress.ShowWindow("Loading projects.");
            Progress.Message = "";
            TaskManager.QueueTask(new Task
            {
                OnJob = delegate
                {
                    foreach (var projectToOpen in ClientSettings.Instance.OpenedProjects)
                    {
                        var project = ProjectsManager.Instance.OpenProject(projectToOpen.Name, projectToOpen.LocalDir, projectToOpen.Host, projectToOpen.Password);
                        TaskManager.DispathSingle(delegate
                        {
                            CreateProjectView(project);
                        });
                    }
                },
                OnDone = delegate
                {
                    Progress.CloseWindow();
                }
            });
        }

        // private
        private void labelNewProject_Click(object sender, System.EventArgs e)
        {
            if (CreateProject.CreateNew() == DialogResult.OK)
            {
                Progress.ShowWindow("Creating...");
                Progress.Message = "Creating project '" + CreateProject.ProjectName + "'...";
                TaskManager.QueueTask(new Task
                {
                    OnJob = delegate
                    {
                        ProjectsManager.Instance.CreateProject(CreateProject.ProjectName, CreateProject.ServerIp, CreateProject.Password); // create project

                        var project = ProjectsManager.Instance.OpenProject(CreateProject.ProjectName, CreateProject.ProjectDirectory, CreateProject.ServerIp, CreateProject.Password);

                        TaskManager.DispathSingle(delegate
                        {
                            CreateProjectView(project);
                        });
                    },
                    OnDone = delegate
                    {
                        Progress.CloseWindow();
                    }
                });
            }
        }
        
        // private
        private void CreateProjectView(Project project)
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