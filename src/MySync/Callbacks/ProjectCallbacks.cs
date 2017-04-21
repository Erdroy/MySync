// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.Collections.Generic;
using System.Linq;
using MySync.Core;
using MySync.Projects;

namespace MySync.Callbacks
{
    /// <summary>
    /// The project callbacks class.
    /// </summary>
    public class ProjectCallbacks
    {
        /// <summary>
        /// JS Callback
        /// </summary>
        public void LoadProjects()
        {
            
        }

        /// <summary>
        /// JS Callback
        /// </summary>
        public void SelectProject(string projectName)
        {
            ProjectManager.Instance.Select(projectName, true);
        }

        /// <summary>
        /// JS Callback
        /// </summary>
        public void Discard()
        {
            Javascript.Run("getSelectedFiles();", files =>
            {
                var data = files as List<object>;

                if (data == null)
                {
                    ProjectManager.Instance.Discard(new string[] { });
                    return;
                }

                ProjectManager.Instance.Discard(data.Select(file => file as string).ToArray());
            });
        }

        /// <summary>
        /// JS Callback
        /// </summary>
        public void Pull()
        {
            ProjectManager.Instance.Pull();
        }

        /// <summary>
        /// JS Callback
        /// </summary>
        public void Push()
        {
            Javascript.Run("getSelectedFiles();", files =>
            {
                var data = files as List<object>;

                if (data == null)
                {
                    ProjectManager.Instance.Push(new string[] { });
                    return;
                }

                ProjectManager.Instance.Push(data.Select(file => file as string).ToArray());
            });
        }
    }
}
