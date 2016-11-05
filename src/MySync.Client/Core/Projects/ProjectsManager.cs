// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using MySync.Client.Utilities;

namespace MySync.Client.Core.Projects
{
    public class ProjectsManager
    {
        private static ProjectsManager _instance;

        internal void Initialize()
        {
            // load projects info
        }

        /// <summary>
        /// Creates new project
        /// </summary>
        /// <param name="client">SFTP client of the target machine.</param>
        /// <param name="projectName">The project name</param>
        /// <exception cref="MySyncException">Handles all exceptions.</exception>
        public Project CreateProject(SFtpClient client, string projectName)
        {
            // send request to the server - to create new project
            return new Project(client, projectName);
        }

        /// <summary>
        /// Opens local working copy of project with given name
        /// </summary>
        /// <param name="client">SFTP client of the target machine.</param>
        /// <param name="projectName">The project name.</param>
        /// <returns>The opened project, null when failed</returns>
        /// <exception cref="MySyncException">Handles all exceptions.</exception>
        public Project OpenProject(SFtpClient client, string projectName)
        {
            // try open project
            return new Project(client, "");
        }

        /// <summary>
        /// The ProjectsManager instance.
        /// </summary>
        public static ProjectsManager Instance => _instance ?? (_instance = new ProjectsManager());
    }
}