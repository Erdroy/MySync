// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ionic.Zip;
using Newtonsoft.Json;

namespace MySync.Shared.VersionControl
{
    /// <summary>
    /// Commit class - holds update data.
    /// </summary>
    public class Commit
    {
        /// <summary>
        /// The commit description.
        /// Default: 'No message' #gitlike
        /// </summary>
        public string Description = "No message";

        /// <summary>
        /// All files in this commit.
        /// </summary>
        public Filemap.FileDiff[] Files;
        
        /// <summary>
        /// Serialze this Commit to json.
        /// </summary>
        /// <returns>The json Commit.</returns>
        public string ToJson()
        {
            // serialize object to JSON
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Add commit to this commit.
        /// </summary>
        /// <param name="commit">The newer commit to add.</param>
        public void Add(Commit commit)
        {
            // build temporary file list
            var files = new List<Filemap.FileDiff>();
            files.AddRange(Files);

            // iterate all file changes
            foreach (var file in commit.Files)
            {
                // try to replace
                if (files.Any(x => x.FileName == file.FileName))
                {
                    // file exists in the file list, update
                    var fidx = files.FindIndex(x => x.FileName == file.FileName);
                    
                    // if file was created and it is not deleted, do not set it as changed
                    if (files[fidx].DiffType == Filemap.FileDiff.Type.Created && file.DiffType != Filemap.FileDiff.Type.Delete)
                        continue;

                    files[fidx] = file;
                    continue;
                }

                // this file does not exists already, add to the list
                files.Add(file);
            }

            // set files array to the latest one
            Files = files.ToArray();
        }

        /// <summary>
        /// Apply commit changes.
        /// </summary>
        /// <param name="projectDir">The project directory.</param>
        /// <param name="dataFile">The commit data file.</param>
        /// <param name="deflate">Unpack the data file?</param>
        public void Apply(string projectDir, string dataFile, bool deflate)
        {
            // apply data
            if (File.Exists(dataFile) && deflate)
            {
                using (var zip = new ZipFile(dataFile))
                {
                    zip.ExtractAll(projectDir, ExtractExistingFileAction.OverwriteSilently);
                }
            }

            // delete files
            foreach (var file in Files)
            {
                var fileName = projectDir + "/" + file.FileName;
                if (file.DiffType == Filemap.FileDiff.Type.Delete)
                {
                    File.Delete(fileName);
                }
                else
                {
#if !SERVER
                    // Apply file modification time,
                    // but only on client.
                    // This is already done by the zip file extraction, 
                    // but there may be some critical issues that will need this. 
                    // Just do it and prevent them all.
                    File.SetLastWriteTime(fileName, DateTime.FromBinary(file.Version));
#endif
                }
            }
        }

        /// <summary>
        /// Check if the upload is really needed.
        /// This will indicate true when there is no any CREATED or CHANGED files, 
        /// but there may be DELETED files because they do not need upload.
        /// </summary>
        /// <returns>True when upload is needed.</returns>
        public bool IsUploadNeeded()
        {
            foreach (var file in Files)
            {
                if (file.DiffType != Filemap.FileDiff.Type.Delete)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Build commit data file.
        /// </summary>
        /// <returns>The data file path.</returns>
        public string Build(string rootDir, string tempFile)
        {
            // delete the file when exists.
            if (File.Exists(tempFile))
                File.Delete(tempFile);

            // compress all files
            using (var zip = new ZipFile(tempFile))
            {
                zip.ParallelDeflateThreshold = -1;

                foreach (var file in Files)
                {
                    if (file.DiffType == Filemap.FileDiff.Type.Delete)
                        continue;

                    var entry = zip.AddFile(rootDir + file.FileName); // add file

                    // change name
                    entry.FileName = file.FileName;
                }

                // save
                zip.Save();
            }

            return tempFile;
        }

        /// <summary>
        /// Create commit from diff.
        /// </summary>
        /// <param name="diff">The files diff.</param>
        /// <param name="desc">(optional)The commit description.</param>
        /// <returns>The created commit.</returns>
        public static Commit FromDiff(Filemap.FileDiff[] diff, string desc = "No message")
        {
            var commit = new Commit
            {
                Files = diff
            };
            return commit;
        }

        /// <summary>
        /// Create Commit from json string.
        /// </summary>
        /// <param name="json">The json string.</param>
        /// <returns>The built Commit.</returns>
        public static Commit FromJson(string json)
        {
            // deserialize object from `json` string.
            return JsonConvert.DeserializeObject<Commit>(json);
        }
    }
}
