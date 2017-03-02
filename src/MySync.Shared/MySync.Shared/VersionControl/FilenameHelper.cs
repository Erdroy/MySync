// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;

namespace MySync.Shared.VersionControl
{
    /// <summary>
    /// FilenameHelper class.
    /// </summary>
    public static class FilenameHelper
    {
        /// <summary>
        /// Convert filename to server-storage style.
        /// </summary>
        public static string ToStorageName(string fileName)
        {
            var data = Uri.EscapeUriString(fileName);
            data = data.Replace("&", "%101");
            data = data.Replace("^", "%102");
            data = data.Replace("$", "%103");
            data = data.Replace("*", "%104");
            data = data.Replace("!", "%105");
            data = data.Replace("@", "%106");

            return data;
        }
    }
}