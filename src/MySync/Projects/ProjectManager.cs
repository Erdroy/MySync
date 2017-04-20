﻿// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.Collections.Generic;
using System.IO;
using System.Linq;
using MySync.Client.Core;
using MySync.Core;
using Newtonsoft.Json;

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
            if (File.Exists("client.json"))
            {
                var config = JsonConvert.DeserializeObject<ClientSettings>(File.ReadAllText("client.json"));

                foreach (var project in config.Projects)
                {
                    var projInst = Project.OpenWorkingCopy(project.Address, project.Name, project.RootDir);
                    
                    projInst.Refresh();
                    var diff = projInst.BuildDiff();

                    Javascript.Run("addProject('" + project.Name + "');");
                    Javascript.Run("setChangeCount('" + project.Name + "', " + diff.Length + ");");
                    
                    AllProjects.Add(projInst);
                }

                // select
                if(!string.IsNullOrEmpty(config.Selected))
                    Select(config.Selected);
            }
        }

        /// <summary>
        /// Selects project by name, also refreshes the files changes.
        /// </summary>
        /// <param name="projectName">The project name.</param>
        /// <param name="callback">Is this javascript callback?</param>
        public void Select(string projectName, bool callback = false)
        {
            try
            {
                // select
                CurrentProject = AllProjects.FirstOrDefault(project => project.ProjectName == projectName);

                if (CurrentProject == null)
                {
                    ClientUI.ShowMessage("Failed to select project '" + projectName + "', invalid project name!", true);
                    return;
                }

                if (!callback)
                    Javascript.Run("selectProject('" + projectName + "', false);");

                CurrentProject.Refresh();
                var diff = CurrentProject.BuildDiff();

                var filesJs = diff.Aggregate("", (current, file) => current + ("addFileChange('" + file.FileName + "', " + (int) file.DiffType) + ");");

                Javascript.Run(filesJs);
                Javascript.Run("setChangeCount('" + CurrentProject.ProjectName + "', " + diff.Length + ");");
            }
            catch
            {
                ClientUI.ShowMessage("Failed to select project '" + projectName + "'!", true);
            }
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
        public List<Project> AllProjects { get; } = new List<Project>();

        /// <summary>
        /// The current instance of project manager.
        /// </summary>
        public static ProjectManager Instance => _instance ?? (_instance = new ProjectManager());
        private static ProjectManager _instance;
    }
}
