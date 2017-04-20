// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.Collections.Generic;
using MySync.Client.Core;

namespace MySync.Projects
{
    /// <summary>
    /// The project manager class.
    /// </summary>
    public class ProjectManager
    {

        public void LoadAll()
        {
            
        }
        
        public void Select(string projectName)
        {

        }

        public void Pull()
        {

        }
        
        public void Push(string[] files)
        {

        }

        public void Discard(string[] files)
        {

        }

        /// <summary>
        /// The current project.
        /// </summary>
        public Project CurrentProject { get; private set; }
        
        /// <summary>
        /// Contains all available projects.
        /// </summary>
        public List<Project> AllProjects { get; private set; }

        /// <summary>
        /// The current instance of project manager.
        /// </summary>
        public static ProjectManager Instance => _instance ?? (_instance = new ProjectManager());
        private static ProjectManager _instance;
    }
}
