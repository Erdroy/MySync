// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.IO;
using MySync.Client.Core.Projects;

namespace MySync.Client.Utilities
{
    public static class PathUtils
    {
        public static string Encode(string file)
        {
            var data = Uri.EscapeUriString(file);
            data = data.Replace("&", "%101");
            data = data.Replace("^", "%102");
            data = data.Replace("$", "%103");
            data = data.Replace("*", "%104");
            data = data.Replace("!", "%105");
            data = data.Replace("@", "%106");
            return data;
        }

        public static string GetPath(string file)
        {
            var i = file.Length - 1;
            while (true)
            {
                if (i < 5)
                    return "";

                if (file[i] == '/' || file[i] == '\\')
                    break;

                i--;
            }
            return file.Substring(0, i);
        }

        public static bool IsExcluded(Project project, string file)
        {
            file = file.Replace("\\", "/");
            file = file.Replace(project.LocalDirectory + "/data/", "");

            var fileName = Path.GetFileName(file);

            if (fileName == ".ignore")
                return false;

            if (project.Exclusions == null)
                return false;
            
            foreach (var exclusion in project.Exclusions)
            {
                if (exclusion.EndsWith("/"))
                {
                    // path
                    if (file.StartsWith(exclusion))
                        return true;
                }
                else if (exclusion.StartsWith("*"))
                {
                    // extension
                    var ext = exclusion.Remove(0, 1);

                    if (fileName.EndsWith(ext))
                        return true;
                }
                else if (exclusion.EndsWith("*"))
                {
                    // filena*
                    var ext = exclusion.Remove(exclusion.Length-1, 1);

                    if (fileName.StartsWith(ext))
                        return true;
                }
                else
                {
                    // filename
                    if (fileName == exclusion)
                        return true;
                }
            }

            return false;
        }
    }
}