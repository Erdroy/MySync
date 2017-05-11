// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using MySync.Client.Core;
using MySync.Core;
using MySync.Shared.RequestHeaders;
using MySync.Shared.Utilities;
using MySync.Shared.VersionControl;
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
            ClientUI.ShowProgress("Loading...");

            if (File.Exists("client.json"))
            {
                var config = JsonConvert.DeserializeObject<ClientSettings>(File.ReadAllText("client.json"));

                var done = 0;
                foreach (var project in config.Projects)
                {
                    var projInst = Project.OpenWorkingCopy(project.Address, project.Name, project.RootDir);
                    projInst.Authority = new ProjectAuthority
                    {
                        ProjectName = project.Name,
                        Username = project.Username,
                        AccessToken = PasswordHasher.Hash(project.Username, project.Password)
                    };

                    Javascript.Run("addProject('" + project.Name + "');");

                    projInst.Refresh(delegate
                    {
                        var diff = projInst.BuildDiff();

                        Javascript.Run("setChangeCount('" + project.Name + "', " + diff.Length + ");");

                        AllProjects.Add(projInst);

                        done++;

                        if (done == config.Projects.Length)
                        {
                            // select
                            if (!string.IsNullOrEmpty(config.Selected))
                                Select(config.Selected); // this will also hide the progress
                            else
                                ClientUI.HideProgress();
                        }
                    });
                }
            }
        }

        /// <summary>
        /// Selects project by name, also refreshes the files changes.
        /// </summary>
        /// <param name="projectName">The project name.</param>
        /// <param name="callback">Is this javascript callback?</param>
        /// <param name="refresh">Refresh the project?</param>
        public void Select(string projectName, bool callback = false, bool refresh = true)
        {
            try
            {
                ClientUI.ShowProgress("Loading...");

                // select
                CurrentProject = AllProjects.FirstOrDefault(project => project.ProjectName == projectName);

                if (CurrentProject == null)
                {
                    ClientUI.ShowMessage("Failed to select project '" + projectName + "', invalid project name!", true);
                    return;
                }

                if (!callback)
                    Javascript.Run("selectProject('" + projectName + "', false);");

                if (refresh)
                {
                    CurrentProject.Refresh(delegate
                    {
                        var diff = CurrentProject.BuildDiff();
                        var filesJs = diff.Aggregate("", (current, file) => current + ("addFileChange('" + file.FileName + "', " + (int) file.DiffType) + ");");

                        Javascript.Run(filesJs);
                        Javascript.Run("setChangeCount('" + CurrentProject.ProjectName + "', " + diff.Length + ");");
                        ClientUI.HideProgress();
                    });
                }
            }
            catch
            {
                if (refresh)
                    ClientUI.HideProgress();
                
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

            try
            {
                ClientUI.ShowProgress("Pulling changes...");
                CurrentProject.Pull(x =>
                {
                    ClientUI.SetProgress("Pulling commit... " + x);
                });
                ClientUI.HideProgress();
                ClientUI.ShowMessage("Pulling done!");
            }
            catch (WarningException ex)
            {
                ClientUI.HideProgress();
                ClientUI.ShowMessage(ex.Message);
            }
            catch (Exception ex)
            {
                ClientUI.HideProgress();
                ClientUI.ShowMessage("Error when pulling, no changes were pulled, message: <br>" + ex.Message, true);
            }
        }

        /// <summary>
        /// Pueshes selected changes from the selected project.
        /// </summary>
        public void Push(string[] files)
        {
            if (CheckProjects())
                return;

            if (files.Length == 0)
            {
                ClientUI.ShowMessage("No file changes selected, select some.");
                return;
            }

            try
            {
                ClientUI.ShowProgress("Building commit...");

                var diffs = CurrentProject.BuildDiff();
                var diff = diffs.Where(file => files.Any(x => x == file.FileName)).ToList();

                if (diff.Count == 0)
                {
                    ClientUI.ShowMessage("No file changes selected, select some.");
                    return;
                }

                var commit = Commit.FromDiff(diff.ToArray());
                var datafile = commit.Build(CurrentProject.RootDir, CurrentProject.RootDir + ".mysync\\commit.zip",
                    x =>
                    {
                        ClientUI.SetProgress("Building commit... " + x );
                    });
                ClientUI.SetProgress("Pushing " + diff.Count + " change(s).");
                CurrentProject.Push(commit, datafile,
                    x =>
                    {
                        ClientUI.SetProgress("Pushing " + diff.Count + " change(s). " + x);
                    });
                ClientUI.HideProgress();
                ClientUI.ShowMessage("Push done!");
            }
            catch (Exception ex)
            {
                ClientUI.HideProgress();
                ClientUI.ShowMessage("Error when pushing, no changes were pushed, message: <br>" + ex.Message, true);
            }
        }

        /// <summary>
        /// Discards selected changes from the selected project.
        /// </summary>
        public void Discard(string[] files)
        {
            if (CheckProjects())
                return;

            ClientUI.ShowProgress("Discarding...");

            try
            {
                var diffs = CurrentProject.BuildDiff();
                var diff = diffs.Where(file => files.Any(x => x == file.FileName)).ToList();
                var commit = Commit.FromDiff(diff.ToArray());

                CurrentProject.Discard(commit.Files,
                    x =>
                    {
                        ClientUI.SetProgress("Discarding " + diff.Count + " change(s). " + x);
                    });
            }
            catch (Exception ex)
            {
                ClientUI.ShowMessage("Error when discarding, no changes were discared, message: <br>" + ex.Message, true);
            }

            ClientUI.HideProgress();
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

