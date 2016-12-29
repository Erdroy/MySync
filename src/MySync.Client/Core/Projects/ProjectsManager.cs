// MySync © 2016 Damian 'Erdroy' Korczowski

using System.IO;
using System.Linq;

namespace MySync.Client.Core.Projects
{
    public class ProjectsManager
    {
        private static ProjectsManager _instance;
        
        internal void Initialize()
        {
        }

        /// <summary>
        /// Creates new project
        /// </summary>
        /// <param name="projectName">The project name</param>
        /// <param name="address">The project's server IP address.</param>
        /// <param name="password">The project's server password for user 'mysync'.</param>
        /// <exception cref="MySyncException">Handles all exceptions.</exception>
        public void CreateProject(string projectName, string address, string password)
        {
            var mainDir = "/home/mysync";

            var client = new SFtpClient(password, address);

            // send request to the server - to create new project
            var rootdir = client.Execute("ls " + mainDir);

            if (rootdir.Length <= 1)
            {
                // this is the first run, create file structure
                client.Execute("mkdir projects; echo \"{\n\n}\" > mysync_config.json");
            }

            client.Execute("cd " + mainDir);

            var projects = client.Execute("ls " + mainDir + "/projects");

            if (projects.Length > 1)
            {
                // parse
                var projectNames = projects.Split('\n');

                if (projectNames.Any(name => name == projectName))
                {
                    UI.Message.ShowMessage("Error", "This project name is already used!");
                    return;
                }
            }

            // create project
            var projectDir = mainDir + "/projects/" + projectName;
            client.Execute("mkdir " + projectDir);

            client.Execute("mkdir " + projectDir + "/commits; " +
                           "mkdir " + projectDir + "/data; " +
                           "mkdir " + projectDir + "/filemaps; ");
        }

        /// <summary>
        /// Opens local working copy of project with given name
        /// </summary>
        /// <param name="projectName">The project name.</param>
        /// <param name="localDirectory">The local output directory.</param>
        /// <param name="address">The project's server IP address.</param>
        /// <param name="password">The project's server password for user 'mysync'.</param>
        /// <returns>The opened project, null when failed</returns>
        /// <exception cref="MySyncException">Handles all exceptions.</exception>
        public Project OpenProject(string projectName, string localDirectory, string address, string password)
        {
            var mainDir = "/home/mysync";
            var projectDir = mainDir + "/projects/" + projectName;

            var localFiles = Directory.GetFiles(localDirectory);

            if (localFiles.Length < 2)
            {
                Directory.CreateDirectory(localDirectory + "\\data");
                Directory.CreateDirectory(localDirectory + "\\commits");
                File.WriteAllText(localDirectory + "\\config.json", @"{}");
            }

            // try open project
            var client = new SFtpClient(password, address);
            return new Project(client, projectName, localDirectory, projectDir);
        }

        /// <summary>
        /// The ProjectsManager instance.
        /// </summary>
        public static ProjectsManager Instance => _instance ?? (_instance = new ProjectsManager());
    }
}