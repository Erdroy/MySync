// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.IO;

namespace MySync.Shared.Utilities
{
    public static class DirectoryHelper
    {
        public static string FilePathToDirectory(string filePath)
        {
            return Path.GetDirectoryName(filePath);
        }

        public static void DeleteIfEmpty(string path, bool goUp = true)
        {
            var directory = FilePathToDirectory(path);
            var dirInfo = new DirectoryInfo(directory);

            DeleteIfEmpty(dirInfo, goUp);
        }

        private static void DeleteIfEmpty(DirectoryInfo info, bool goUp)
        {
            if (info.GetFiles().Length == 0 && info.GetDirectories().Length == 0)
            {
                info.Delete();

                if (goUp)
                {
                    DeleteIfEmpty(info.Parent, true);
                }
            }
        }
    }
}
