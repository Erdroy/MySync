// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.Linq;

namespace MySync.Shared.Utilities
{
    /// <summary>
    /// File utils class.
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Checks if file is excluded using exclusion list.
        /// </summary>
        /// <param name="fileName">The file to check.</param>
        /// <param name="exclusions">The exclusion list.</param>
        /// <returns>True when file is excluded.</returns>
        public static bool IsExcluded(string fileName, string[] exclusions)
        {
            fileName = fileName.Replace("\\", "/");

            foreach (var exclude in exclusions)
            {
                if (exclude.Length < 2) // validate
                    continue;

                // check directory exclude
                if ((exclude.EndsWith("/") || exclude.EndsWith("*")) && fileName.StartsWith(exclude.Replace("*", "")))
                    return true;

                // check extension exclude
                if (exclude.StartsWith("*.") && fileName.EndsWith(exclude.Replace("*", "")))
                    return true;

                // check file exclude
                if (fileName == exclude)
                    return true;
            }

            return false;
        }
    }
}
