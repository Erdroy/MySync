// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace MySync.Shared.VersionControl
{
    /// <summary>
    /// Filemap class.
    /// </summary>
    public class Filemap : IDisposable
    {
        /// <summary>
        /// Filemap.File structure.
        /// </summary>
        public struct File
        {
            public string FileName;
            public long Version;
        }

        /// <summary>
        /// Filemap.FileDiff structure.
        /// </summary>
        public struct FileDiff
        {
            /// <summary>
            /// Filemap.FileDiff.Type structure.
            /// </summary>
            public enum Type
            {
                Created,
                Changed,
                Delete,
            }

            public string FileName;
            public Type DiffType;
            public long Version;
        }

        // private
        private List<File> _files = new List<File>();

        // private
        private Filemap() { }

        /// <summary>
        /// Check if file is present in this filemap.
        /// </summary>
        /// <param name="filename">The file map.</param>
        /// <returns>True when file is present.</returns>
        public bool IsPresent(string filename)
        {
            return _files.Any(file => file.FileName == filename);
        }

        /// <summary>
        /// Check if file is different that one stored in this filemap.
        /// WARNING: returns `false` when file is not found!
        /// </summary>
        /// <param name="file">The file entry.</param>
        /// <returns>True when file is changed. Falsed when file is not changed or not found.</returns>
        public bool IsChanged(File file)
        {
            return _files.Any(current => current.FileName == file.FileName && current.Version != file.Version);
        }

        /// <summary>
        /// Calculate diff between this filemap and given filemap.
        /// </summary>
        /// <param name="filemap">The filemap which is the 'latest' filemap.</param>
        /// <returns>The file diff map.</returns>
        public FileDiff[] GetDiff(Filemap filemap)
        {
            var diff = new List<FileDiff>();

            // find new files of modified
            foreach (var file in filemap._files)
            {
                if (!IsPresent(file.FileName))
                {
                    // this file has been created
                    // add to diff map
                    diff.Add(new FileDiff
                    {
                        FileName = file.FileName,
                        DiffType = FileDiff.Type.Created,
                        Version = file.Version
                    });
                    continue;
                }

                if (IsChanged(file))
                {
                    // this file is changed
                    // add to diff map
                    diff.Add(new FileDiff
                    {
                        FileName = file.FileName,
                        DiffType = FileDiff.Type.Changed,
                        Version = file.Version
                    });
                }
            }

            // find deleted files
            foreach (var file in _files)
            {
                if (!filemap.IsPresent(file.FileName))
                {
                    // this file is deleted
                    // add to diff map
                    diff.Add(new FileDiff
                    {
                        FileName = file.FileName,
                        DiffType = FileDiff.Type.Delete
                    });
                }
            }

            return diff.ToArray();
        }

        /// <summary>
        /// Add changes.
        /// </summary>
        /// <param name="root">The project root directory.</param>
        /// <param name="fileDiffs">The files diff.</param>
        public void AddChanges(string root, FileDiff[] fileDiffs)
        {
            if (!root.EndsWith("\\"))
                root += "\\";

            foreach (var file in fileDiffs)
            {
                if (file.DiffType == FileDiff.Type.Created)
                {
                    // add file
                    var fileName = root + file.FileName;
                    var fileInfo = new FileInfo(fileName);
                    _files.Add(new File
                    {
                        FileName = file.FileName,
                        Version = fileInfo.LastWriteTime.ToBinary()
                    });

                    // next, please!
                    continue;
                }

                // remove file
                if (file.DiffType == FileDiff.Type.Delete)
                    _files.Remove(_files.FirstOrDefault(x => x.FileName == file.FileName));
            }
        }

        /// <summary>
        /// Serialze this filemap to json.
        /// </summary>
        /// <returns>The json filemap.</returns>
        public string ToJson()
        {
            // serialize object to JSON
            return JsonConvert.SerializeObject(_files.ToArray(), Formatting.Indented);
        }

        /// <summary>
        /// Dispose this filemap.
        /// </summary>
        public void Dispose()
        {
            _files.Clear();
        }

        /// <summary>
        /// Create filemap from json string.
        /// </summary>
        /// <param name="json">The json string.</param>
        /// <returns>The built filemap.</returns>
        public static Filemap FromJson(string json)
        {
            // deserialize object from `json` string.
            var filemap = new Filemap
            {
                _files = new List<File>(JsonConvert.DeserializeObject<File[]>(json))
            };
            return filemap;
        }

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

            // find all files
            var files = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories);

            // build file version map
            foreach (var file in files)
            {
                var filename = file.Replace("\\", "/").Remove(0, root.Length);

                if(filename.Contains(".mysync"))
                    continue;

                // TODO: check exclusions

                // check file info
                var fileinfo = new FileInfo(file);
                filemap._files.Add(new File
                {
                    FileName = filename,
                    Version = fileinfo.LastWriteTime.ToBinary()
                });
            }

            return filemap;
        }

        /// <summary>
        /// Build empty filemap.
        /// </summary>
        /// <returns>The built filemap.</returns>
        public static Filemap BuildEmpty()
        {
            return new Filemap();
        }
    }
}
