// MySync © 2016-2017 Damian 'Erdroy' Korczowski


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MySync.Client.Core.Projects;
using MySync.Client.Utilities;
using Newtonsoft.Json;

namespace MySync.Client.Core
{
    public class FileMapping
    {
        public struct FileEntry : IEquatable<FileEntry>
        {
            public string File;
            public long Version;

            public bool Equals(FileEntry other)
            {
                return File == other.File;
            }
        }

        public List<FileEntry> Files = new List<FileEntry>();

        public void Update(Project project, string rootDirectory)
        {
            var files = Directory.EnumerateFiles(rootDirectory, "*.*", SearchOption.AllDirectories);

            Files.Clear();
            foreach (var file in files)
            {
                // ignore files

                if (PathUtils.IsExcluded(project, file))
                    continue;

                var fileinfo = new FileInfo(file);
                
                var filePath = file;
                filePath = filePath.Remove(0, rootDirectory.Length);

                Files.Add(new FileEntry
                {
                    File = filePath.Replace("\\", "/"),
                    Version = fileinfo.LastWriteTime.ToBinary()
                });
            }
        }
        
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public FileMapping Exclude(List<Commit.CommitEntry> exclude)
        {
            var vmap = new FileMapping();
            vmap.Files.AddRange(Files);
            
            vmap.Files.RemoveAll(x => exclude.Any(y => x.File == y.Entry));

            return vmap;
        }
        
        public static FileMapping CreateFileMapping(Project project, string rootDirectory)
        {
            var mapping = new FileMapping();
            
            // TODO: Some optimizations? Run in other thread, maybe show progress bar
            var files = Directory.EnumerateFiles(rootDirectory, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                // ignore files

                if (PathUtils.IsExcluded(project, file))
                    continue;

                var fileinfo = new FileInfo(file);

                var filePath = file;
                filePath = filePath.Remove(0, rootDirectory.Length);

                mapping.Files.Add(new FileEntry
                {
                    File = filePath.Replace("\\", "/"),
                    Version = fileinfo.LastWriteTime.ToBinary()
                });
            }

            return mapping;
        }

        public static List<FileEntry> GetChangedFiles(FileMapping localMapping, FileMapping remoteMapping)
        {
            return localMapping.Files.Where(local => remoteMapping.Files.Any(remote => local.File == remote.File && DateTime.FromBinary(local.Version) > DateTime.FromBinary(remote.Version))).ToList();
        }

        public static List<FileEntry> GetNewFiles(FileMapping localMapping, FileMapping remoteMapping)
        {
            var changedFiles = GetChangedFiles(localMapping, remoteMapping);
            var newFiles = localMapping.Files.Except(remoteMapping.Files).ToList();

            // remove changed files
            foreach (var file in changedFiles)
            {
                if (newFiles.Contains(file))
                    newFiles.Remove(file);
            }

            return newFiles;
        }

        public static List<FileEntry> GetDeletedFiles(FileMapping localMapping, FileMapping remoteMapping)
        {
            var newFiles = GetNewFiles(localMapping, remoteMapping);
            var deletedFiles = remoteMapping.Files.Except(localMapping.Files).ToList();
            
            // remove changed files
            foreach (var file in newFiles)
            {
                if (deletedFiles.Contains(file))
                    deletedFiles.Remove(file);
            }

            return deletedFiles;
        }

        public static Commit.CommitEntry[] BuildEntries(Project project)
        {
            var commitId = project.GetCurrentCommit();
            return BuildEntries(project.FileSystem.GetLocalMapping(), project.FileSystem.GetRemoteMapping(commitId));
        }

        public static Commit.CommitEntry[] BuildEntries(FileMapping localMapping, FileMapping remoteMapping)
        {
            List<Commit.CommitEntry> files;

            if (remoteMapping == null)
            {
                files = new List<Commit.CommitEntry>();

                foreach (var file in localMapping.Files)
                {
                    if(file.File == ".ignore")
                        continue;

                    files.Add(new Commit.CommitEntry(CommitEntryType.Created, file.File));
                }

                return files.ToArray();
            }

            var changedFiles = GetChangedFiles(localMapping, remoteMapping);
            var newFiles = GetNewFiles(localMapping, remoteMapping);
            var deletedFiles = GetDeletedFiles(localMapping, remoteMapping);

            files = changedFiles.Select(file => new Commit.CommitEntry(CommitEntryType.Changed, file.File)).ToList();
            files.AddRange(newFiles.Select(file => new Commit.CommitEntry(CommitEntryType.Created, file.File)));
            files.AddRange(deletedFiles.Select(file => new Commit.CommitEntry(CommitEntryType.Deleted, file.File)));
            
            return files.ToArray();
        }

        public static FileMapping FromJsonFile(string jsonfile)
        {
            return JsonConvert.DeserializeObject<FileMapping>(File.ReadAllText(jsonfile));
        }

        public static FileMapping FromJson(string json)
        {
            return JsonConvert.DeserializeObject<FileMapping>(json);
        }
    }
}