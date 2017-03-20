// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Collections.Generic;

namespace MySync.Server.Core
{
    public static class ProjectLock
    {
        /// <summary>
        /// The lock mode enum.
        /// </summary>
        public enum LockMode
        {
            None,

            Upload,
            Any
        }

        /// <summary>
        /// Contains all locked projects.
        /// </summary>
        public static readonly Dictionary<string, LockMode> LockedProjects = new Dictionary<string, LockMode>();

        /// <summary>
        /// Try to lock specified project.
        /// </summary>
        /// <param name="project">The project name.</param>
        /// <param name="mode">The lock mode.</param>
        /// <returns>The lock mode.</returns>
        public static LockMode TryLock(string project, LockMode mode)
        {
            if (LockedProjects.ContainsKey(project))
            {
                return LockedProjects[project];
            }

            Console.WriteLine("Lock: " + project);
            LockedProjects.Add(project, mode);
            return LockMode.None;
        }

        /// <summary>
        /// Unlock specified project.
        /// </summary>
        /// <param name="project">The project name.</param>
        public static void Unlock(string project)
        {
            if (LockedProjects.ContainsKey(project))
                LockedProjects.Remove(project);

            Console.WriteLine("Unlock: " + project);
        }
    }
}