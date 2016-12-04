// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MySync.Client.Core.Projects;
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

        public void Update(string rootDirectory)
        {
            var files = Directory.EnumerateFiles(rootDirectory, "*.*", SearchOption.AllDirectories);

            Files.Clear();
            foreach (var file in files)
            {
                var fileinfo = new FileInfo(file);
                Files.Add(new FileEntry
                {
                    File = file,
                    Version = fileinfo.LastWriteTime.ToBinary()
                });
            }
        }
        
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static FileMapping CreateFileMapping(string rootDirectory)
        {
            var mapping = new FileMapping();

            // TODO: Some optimizations? Caching or something?
            // TODO: Run in other thread, maybe show progress bar
            var files = Directory.EnumerateFiles(rootDirectory, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
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
            return localMapping.Files.Where(p => remoteMapping.Files.Any(l => p.File == l.File && p.Version != l.Version)).ToList();
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
            return BuildEntries(project.FileSystem.GetLocalMapping(), project.FileSystem.GetRemoteMapping());
        }

        public static Commit.CommitEntry[] BuildEntries(FileMapping localMapping, FileMapping remoteMapping)
        {
            var changedFiles = GetChangedFiles(localMapping, remoteMapping);
            var newFiles = GetNewFiles(localMapping, remoteMapping);
            var deletedFiles = GetDeletedFiles(localMapping, remoteMapping);

            var files = changedFiles.Select(file => new Commit.CommitEntry(CommitEntryType.Changed, file.File)).ToList();
            files.AddRange(newFiles.Select(file => new Commit.CommitEntry(CommitEntryType.Created, file.File)));
            files.AddRange(deletedFiles.Select(file => new Commit.CommitEntry(CommitEntryType.Deleted, file.File)));

            return files.ToArray();
        }

        public static FileMapping FromJson(string jsonfile)
        {
            return JsonConvert.DeserializeObject<FileMapping>(File.ReadAllText(jsonfile));
        }
    }
}