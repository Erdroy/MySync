// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.Collections.Generic;
using MySync.Client.Core;
using MySync.Core;

namespace MySync.Projects
{
    /// <summary>
    /// The project manager class.
    /// </summary>
    public class ProjectManager
    {
        /// <summary>
        /// Loads all available projects and selects last opened project.
        /// </summary>
        public void LoadAll()
        {
            
        }
        
        /// <summary>
        /// Selects project by name, also refreshes the files changes.
        /// </summary>
        /// <param name="projectName">The project name.</param>
        public void Select(string projectName)
        {
            if (CheckProjects())
                return;

        }

        /// <summary>
        /// Pulls all changes on the selected project.
        /// </summary>
        public void Pull()
        {
            if (CheckProjects())
                return;

        }

        /// <summary>
        /// Pueshes selected changes from the selected project.
        /// </summary>
        public void Push(string[] files)
        {
            if (CheckProjects())
                return;

        }

        /// <summary>
        /// Discards selected changes from the selected project.
        /// </summary>
        public void Discard(string[] files)
        {
            if (CheckProjects())
                return;
        }

        // private
        private bool CheckProjects()
        {
            if (CurrentProject == null || AllProjects.Count == 0)
            {
                ClientUI.ShowMessage("You have no any project, create or open new one.", true);
                return true;
            }

            return false;
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
