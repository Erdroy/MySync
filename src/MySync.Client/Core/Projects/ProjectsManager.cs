// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

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
        public Project CreateProject(string projectName)
        {
            // send request to the server - to create new project
            return new Project(Client, projectName);
        }

        /// <summary>
        /// Opens local working copy of project with given name
        /// </summary>
        /// <param name="projectName">The project name.</param>
        /// <returns>The opened project, null when failed</returns>
        /// <exception cref="MySyncException">Handles all exceptions.</exception>
        public Project OpenProject(string projectName)
        {
            // try open project
            return new Project(Client, "");
        }

        /// <summary>
        /// The ProjectsManager instance.
        /// </summary>
        public static ProjectsManager Instance => _instance ?? (_instance = new ProjectsManager());
    }
}