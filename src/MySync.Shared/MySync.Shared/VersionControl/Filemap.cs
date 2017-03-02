﻿// MySync © 2016-2017 Damian 'Erdroy' Korczowski

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
        }

        // private
        private readonly List<File> _files = new List<File>();

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
                        DiffType = FileDiff.Type.Created
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
                        DiffType = FileDiff.Type.Changed
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
        /// Serialze this filemap to json.
        /// </summary>
        /// <returns>The json filemap.</returns>
        public string ToJson()
        {
            // serialize this object to JSON
            return JsonConvert.SerializeObject(this, Formatting.Indented);
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
            return JsonConvert.DeserializeObject<Filemap>(json);
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
        /// Build empty filemap for testing.
        /// </summary>
        /// <returns>The built filemap.</returns>
        public static Filemap BuildFree()
        {
            var filemap = new Filemap();
            filemap._files.Add(new File
            {
                FileName = "/test.txt",
                Version = 0
            });
            return filemap;
        }
    }
}