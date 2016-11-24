// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace MySync.Client.Core
{
    public class FileMapping
    {
        public struct FileEntry
        {
            public string File;
            public long Version;
        }

        public List<FileEntry> Files = new List<FileEntry>();

        public static FileMapping CreateFileMapping(string rootDirectory)
        {
            var mapping = new FileMapping();

            // TODO: Some optimizations? Caching or something?
            // TODO: Run in other thread, maybe show progress bar
            var files = Directory.EnumerateFiles(rootDirectory, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var fileinfo = new FileInfo(file);
                mapping.Files.Add(new FileEntry
                {
                    File = file,
                    Version = fileinfo.LastWriteTime.ToBinary()
                });
            }

            return mapping;
        }

        public static FileMapping FromJson(string jsonfile)
        {
            return JsonConvert.DeserializeObject<FileMapping>(File.ReadAllText(jsonfile));
        }
    }
}