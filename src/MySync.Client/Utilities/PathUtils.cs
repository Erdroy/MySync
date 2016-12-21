// MySync © 2016 Damian 'Erdroy' Korczowski


namespace MySync.Client.Utilities
{
    public static class PathUtils
    {
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
    }
}