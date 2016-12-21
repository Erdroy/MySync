// MySync © 2016 Damian 'Erdroy' Korczowski


using System.IO;
using System.Linq;
using System.Windows.Forms;
using MySync.Client.Utilities;

namespace MySync.Client.Core.Projects
{
    public class ProjectsManager
    {
        private static ProjectsManager _instance;

        public static SFtpClient Client;

        internal void Initialize()
        {
            // load projects info
            Client = new SFtpClient(ClientSettings.Instance.Password, ClientSettings.Instance.Host);
        }

        /// <summary>
        /// Creates new project
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <exception cref="MySyncException">Handles all exceptions.</exception>
        public void CreateProject(string projectName)
        {
            // send request to the server - to create new project
            var rootdir = Client.Execute("ls " + ClientSettings.Instance.MainDirectory);

            if (rootdir.Length <= 1)
            {
                // this is the first run, create file structure
                Client.Execute("mkdir projects; echo \"{\n\n}\" > mysync_config.json");
            }

            Client.Execute("cd " + ClientSettings.Instance.MainDirectory);

            var projects = Client.Execute("ls " + ClientSettings.Instance.MainDirectory + "/projects");

            if (projects.Length > 1)
            {
                // parse
                var projectNames = projects.Split('\n');

                if (projectNames.Any(name => name == projectName))
                {
                    MessageBox.Show(@"This project name is already used!");
                    return;
                }
            }

            // create project
            var projectDir = ClientSettings.Instance.MainDirectory + "/projects/" + projectName;
            Client.Execute("mkdir " + projectDir);

            Client.Execute("mkdir " + projectDir + "/commits; " +
                           "mkdir " + projectDir + "/data; " +
                           "mkdir " + projectDir + "/filemaps; " +
                           "echo \"0\"> " + projectDir + "/lockfile; ");
        }

        /// <summary>
        /// Opens local working copy of project with given name
        /// </summary>
        /// <param name="projectName">The project name.</param>
        /// <param name="localDirectory">The local output directory.</param>
        /// <returns>The opened project, null when failed</returns>
        /// <exception cref="MySyncException">Handles all exceptions.</exception>
        public Project OpenProject(string projectName, string localDirectory)
        {
            var projectDir = ClientSettings.Instance.MainDirectory + "/projects/" + projectName;

            var localFiles = Directory.GetFiles(localDirectory);

            if (localFiles.Length < 2)
            {
                Directory.CreateDirectory(localDirectory + "\\data");
                Directory.CreateDirectory(localDirectory + "\\commits");
                File.WriteAllText(localDirectory + "\\config.json", @"{}");
            }
            
            // try open project
            return new Project(Client, projectName, localDirectory, projectDir);
        }

        /// <summary>
        /// The ProjectsManager instance.
        /// </summary>
        public static ProjectsManager Instance => _instance ?? (_instance = new ProjectsManager());
    }
}