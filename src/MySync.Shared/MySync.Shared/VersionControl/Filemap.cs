// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.Collections.Generic;
using System.IO;

namespace MySync.Shared.VersionControl
{
    public class Filemap
    {
        public struct File
        {
            public string FileName;
            public long Version;
        }

        // private
        private List<File> _files = new List<File>();

        // private
        private Filemap() { }
        
        /// <summary>
        /// Build filemap for `root` directory.
        /// </summary>
        /// <param name="root">The root directory.</param>
        /// <returns>The built filemap.</returns>
        public static Filemap Build(string root)
        {
            var filemap = new Filemap();

            if (!Directory.Exists(root))
                throw new DirectoryNotFoundException();

            var files = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                // TODO: check exclusions

                var filename = file.Replace("\\", "/").Remove(0, root.Length);
                
                var fileinfo = new FileInfo(file);
                filemap._files.Add(new File
                {
                    FileName = filename,
                    Version = fileinfo.LastWriteTime.ToBinary()
                });
            }

            return filemap;
        }
    }
}
