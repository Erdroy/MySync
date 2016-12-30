// MySync © 2016 Damian 'Erdroy' Korczowski

using System;
using System.IO;
using System.Linq;
using MySync.Client.Core.Projects;
using MySync.Client.Utilities;

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
                Mapping.Update(Project, rootDir);
                return;
            }

            Mapping = FileMapping.CreateFileMapping(Project, rootDir);

            _fileSystemWatcher?.Dispose();
            _fileSystemWatcher = new FileSystemWatcher(rootDir)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,

                Filter = "*.*",

                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime | 
                NotifyFilters.FileName | NotifyFilters.DirectoryName
            };

            _fileSystemWatcher.Created += FileSystemWatcher_Created;
            _fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            _fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;
            _fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
        }

        public void DeleteEmptyDirs(string dir)
        {
            try
            {
                foreach (var d in Directory.EnumerateDirectories(dir))
                {
                    DeleteEmptyDirs(d);
                }

                var entries = Directory.EnumerateFileSystemEntries(dir);

                if (!entries.Any())
                {
                    try
                    {
                        Directory.Delete(dir);
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }
            }
            catch (UnauthorizedAccessException) { }
        }

        public FileMapping GetRemoteMapping()
        {
            // download from the server
            // load from json
            // return
            var commitId = Project.GetCurrentCommit();
            try
            {
                var json = Client.DownloadFile(Project.RemoteDirectory + "/filemaps/filemap_" + commitId + ".json");
                return FileMapping.FromJson(json);
            }
            catch
            {
                return null;
            }
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
            // ignore files
            if (PathUtils.IsExcluded(Project, e.FullPath))
                return;

            var attributes = File.GetAttributes(e.FullPath);

            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                // this is a directory
                return;
            }

            var filename = e.FullPath.Remove(0, (Project.LocalDirectory + "\\data\\").Length).Replace("\\", "/");
            
            var fileInfo = new FileInfo(e.FullPath);

            Mapping.Files.Add(new FileMapping.FileEntry
            {
                File = filename,
                Version = fileInfo.LastWriteTime.ToBinary()
            });
            Changed = true;
        }

        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            // ignore files
            if (PathUtils.IsExcluded(Project, e.FullPath))
                return;

            var attributes = File.GetAttributes(e.FullPath);

            if ((attributes & FileAttributes.Directory) == FileAttributes.Directory)
            {
                // this is a directory
                BuildFilemap();
                Changed = true;
                return;
            }

            var filename = e.FullPath.Remove(0, (Project.LocalDirectory + "\\data\\").Length).Replace("\\", "/");
            var oldfilename = e.OldFullPath.Remove(0, (Project.LocalDirectory + "\\data\\").Length).Replace("\\", "/");

            var modfile = Mapping.Files.FindIndex(file => file.File == oldfilename);

            if (modfile == -1)
                return;
            
            Mapping.Files[modfile] = new FileMapping.FileEntry
            {
                File = filename,
                Version = new FileInfo(e.FullPath).LastWriteTime.ToBinary()
            };

            Changed = true;
        }

        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            // ignore files 
            if (PathUtils.IsExcluded(Project, e.FullPath))
                return;

            var filename = e.FullPath.Remove(0, (Project.LocalDirectory + "\\data\\").Length).Replace("\\", "/");
            
            var modfile = Mapping.Files.Find(file => file.File == filename);

            if(modfile.File != null)
                Mapping.Files.Remove(modfile);

            Changed = true;
            DeleteEmptyDirs(Project.LocalDirectory + "/data/");
        }

        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            // ignore files
            if (PathUtils.IsExcluded(Project, e.FullPath))
                return;

            // only changed event type can pass
            if (e.ChangeType != WatcherChangeTypes.Changed)
                return;

            var filename = e.FullPath.Remove(0, (Project.LocalDirectory + "\\data\\").Length).Replace("\\", "/");
            
            if (e.Name == ".ignore")
            {
                // dispatch
                TaskManager.DispathSingle(delegate
                {
                    Project.LoadExclusions();
                    BuildFilemap();
                    Changed = true;
                });
                return;
            }

            // find then remove and add new version of file to the mapping
            for (var i = 0; i < Mapping.Files.Count; i++)
            {
                if (Mapping.Files[i].File != filename)
                    continue;

                Mapping.Files.RemoveAt(i);

                Mapping.Files.Add(new FileMapping.FileEntry
                {
                    File = filename,
                    Version = new FileInfo(e.FullPath).LastWriteTime.ToBinary()
                });
                Changed = true;
                return;
            }
        }

        public string[] GetFilesRemote(string offDir)
        {
            return Client.ListFiles(Project.RemoteDirectory + offDir);
        }

        public string[] GetFilesLocal(string offDir)
        {
            var files = Directory.GetFiles(Project.LocalDirectory + offDir);

            return files.Select(Path.GetFileName).ToArray();
        }
        
        public bool Changed { get; set; }

        public SFtpClient Client { get; private set; }

        public FileMapping Mapping { get; private set; }

        public Project Project { get; set; }
    }
}