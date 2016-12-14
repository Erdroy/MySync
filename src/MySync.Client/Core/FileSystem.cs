// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System.IO;
using MySync.Client.Core.Projects;

namespace MySync.Client.Core
{
    public class FileSystem
    {
        private FileSystemWatcher _fileSystemWatcher;
        
        internal FileSystem() { }

        public void Open(SFtpClient client)
        {
            Client = client;
        }

        public void Close()
        {
            Client.Close();
        }

        public void BuildFilemap()
        {
            // create file mapping
            // run file system watcher

            var rootDir = Project.LocalDirectory + "\\data\\";

            if (Mapping != null)
            {
                Changed = false;
                Mapping.Update(rootDir);
                return;
            }

            Mapping = FileMapping.CreateFileMapping(rootDir);

            _fileSystemWatcher?.Dispose();
            _fileSystemWatcher = new FileSystemWatcher(rootDir)
            {
                EnableRaisingEvents = true,

                Filter = "*.*",
                IncludeSubdirectories = true,

                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | 
                NotifyFilters.FileName | NotifyFilters.DirectoryName
            };

            _fileSystemWatcher.Created += FileSystemWatcher_Created;
            _fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            _fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            _fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
        }
        
        public FileMapping GetRemoteMapping()
        {
            // download from the server
            // load from json
            // return

            var remoteFilemapOut = Project.LocalDirectory + "\\remoteFileMap.json";
            Client.DownloadFile(remoteFilemapOut, Project.RemoteDirectory + "/filemap");

            return FileMapping.FromJson(remoteFilemapOut);
        }

        public FileMapping GetLocalMapping()
        {
            return Mapping;
        }

        public void DeleteLocalFile(string file)
        {
            File.Delete(file);
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Mapping.Files.Add(new FileMapping.FileEntry
            {
                File = e.FullPath,
                Version = new FileInfo(e.FullPath).LastWriteTime.ToBinary()
            });
            Changed = true;
        }

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            var modfile = Mapping.Files.Find(file => file.File == e.OldFullPath);

            if (modfile.File != null)
                modfile.File = e.FullPath;
            Changed = true;
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            var modfile = Mapping.Files.Find(file => file.File == e.FullPath);

            if(modfile.File != null)
                Mapping.Files.Remove(modfile);
            Changed = true;
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            // only changed event type can pass
            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            // find then remove and add new version of file to the mapping
            for (var i = 0; i < Mapping.Files.Count; i++)
            {
                if (Mapping.Files[i].File != e.FullPath)
                    continue;

                Mapping.Files.RemoveAt(i);

                Mapping.Files.Add(new FileMapping.FileEntry
                {
                    File = e.FullPath,
                    Version = new FileInfo(e.FullPath).LastWriteTime.ToBinary()
                });
                Changed = true;
                return;
            }
        }

        public string[] ExcludedExtensions { get; private set; }

        public string[] ExcludedFiles { get; private set; }

        public string[] ExcludedDirectories { get; private set; }

        public bool Changed { get; private set; }

        public SFtpClient Client { get; private set; }

        public FileMapping Mapping { get; private set; }

        public Project Project { get; set; }
    }
}