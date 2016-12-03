// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System.IO;
using MySync.Client.Core.Projects;

namespace MySync.Client.Core
{
    public class FileSystem
    {
        public SFtpClient Client { get; private set; }

        public FileMapping Mapping { get; private set; }

        public Project Project { get; set; }

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

            Mapping = FileMapping.CreateFileMapping(Project.LocalDirectory + "\\data\\");

            _fileSystemWatcher?.Dispose();
            _fileSystemWatcher = new FileSystemWatcher(Project.LocalDirectory + "\\data\\")
            {
                EnableRaisingEvents = true,

                Filter = "*.*",

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

            var remoteMapping = FileMapping.FromJson(remoteFilemapOut);
            var localMapping = Mapping.ToJson();

            var changedFiles = FileMapping.GetChangedFiles(Mapping, remoteMapping);
            var newFiles = FileMapping.GetNewFiles(Mapping, remoteMapping);
            var deletedFiles = FileMapping.GetDeletedFiles(Mapping, remoteMapping);

            return null;
        }

        public FileMapping GetLocalMapping()
        {
            return Mapping;
        }

        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Mapping.Files.Add(new FileMapping.FileEntry
            {
                File = e.FullPath,
                Version = new FileInfo(e.FullPath).LastWriteTime.ToBinary()
            });
        }

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            var modfile = Mapping.Files.Find(file => file.File == e.OldFullPath);

            if (modfile.File != null)
                modfile.File = e.FullPath;
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            var modfile = Mapping.Files.Find(file => file.File == e.FullPath);

            if(modfile.File != null)
                Mapping.Files.Remove(modfile);
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
                return;
            }
        }

        public string[] Excluded { get; private set; }
    }
}