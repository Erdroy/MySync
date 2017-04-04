// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.Linq;

namespace MySync.Server.Core
{
    /// <summary>
    /// Authorization class.
    /// </summary>
    public static class Authorization
    {
        /// <summary>
        /// Checks if given token has authority for project.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="project">The project.</param>
        /// <returns>Has authority?</returns>
        public static bool HasAuthority(string token, string project)
        {
            var projectSettings = ServerCore.Settings.Projects.FirstOrDefault(
                x => x.Name == project
            );

            return projectSettings != null && projectSettings.AccessTokens.Contains(token);
        }
    }
}
