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

                    // if file was created and now it is deleted, remove this file from commit
                    if (files[fidx].DiffType == Filemap.FileDiff.Type.Created && file.DiffType == Filemap.FileDiff.Type.Delete)
                    {
                        files.RemoveAt(fidx);
                        continue;
                    }

                    // file was created in some previous commits, not in this pulling, change the file to changed or deleted.
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
        /// Makes commit file backup.
        /// </summary>
        /// <param name="projectDir">The project dir.</param>
        public void Backup(string projectDir)
        {
            var backupDir = projectDir + "/_backups/";
            Directory.CreateDirectory(backupDir);
            foreach (var file in Files)
            {
                if (file.DiffType != Filemap.FileDiff.Type.Created)
                {
                    // copy
                    File.Copy(projectDir + "/" + file.FileName, backupDir + Path.GetFileName(file.FileName), true);
                }
            }
        }

        /// <summary>
        /// Restores commit backup files.
        /// </summary>
        /// <param name="projectDir">The project dir.</param>
        public void RestoreBackup(string projectDir)
        {
            var backupDir = projectDir + "/_backups/";

            foreach (var file in Files)
            {
                if (file.DiffType != Filemap.FileDiff.Type.Created)
                {
                    // copy
                    File.Copy(backupDir + Path.GetFileName(file.FileName), projectDir + "/" + file.FileName, true);
                }
                else
                {
                    // delete if exists
                    if (File.Exists(projectDir + "/" + file.FileName))
                        File.Delete(projectDir + "/" + file.FileName);
                }
            }
            Directory.Delete(backupDir, true);
        }

        /// <summary>
        /// Apply commit changes.
        /// </summary>
        /// <param name="projectDir">The project directory.</param>
        /// <param name="dataFile">The commit data file.</param>
        /// <param name="deflate">Unpack the data file?</param>
        public void Apply(string projectDir, string dataFile, bool deflate)
        {
            // delete files
            foreach (var file in Files)
            {
                var fileName = projectDir + "/" + file.FileName;
                if (file.DiffType == Filemap.FileDiff.Type.Delete)
                {
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch
                    {
                        // ignore
                    }
                }
            }

            // apply data
            if (File.Exists(dataFile) && deflate)
            {
                using (var zip = new ZipFile(dataFile))
                {
                    zip.ExtractAll(projectDir, ExtractExistingFileAction.OverwriteSilently);
                }
            }

            // apply mod time
            foreach (var file in Files)
            {
#if !SERVER
                if (file.DiffType != Filemap.FileDiff.Type.Delete) // apply only for existing files
                {
                    var fileName = projectDir + "/" + file.FileName;

                    // Apply file modification time,
                    // but only on client.
                    // This is already done by the zip file extraction, 
                    // but there may be some critical issues that will need this. 
                    // Just do it and prevent them all.
                    File.SetLastWriteTime(fileName, DateTime.FromBinary(file.Version));
                }
#endif
            }
        }

        /// <summary>
        /// Build commit data file.
        /// </summary>
        /// <returns>The data file path.</returns>
        public string Build(string rootDir, string tempFile, Action<int> onProgress = null)
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

                if (onProgress != null)
                {
                    zip.SaveProgress += (sender, args) =>
                    {
                        var progress = (int)((float)args.BytesTransferred / args.TotalBytesToTransfer * 100.0f);
                        onProgress(progress);
                    };
                }

                // save
                zip.Save();
            }

            return tempFile;
        }
        
        /// <summary>
        /// Check if the upload is really needed.
        /// This will indicate true when there is no any CREATED or CHANGED files, 
        /// but there may be DELETED files because they do not need upload.
        /// </summary>
        /// <returns>True when upload is needed.</returns>
        public bool IsUploadNeeded()
        {
            return Files.Any(file => file.DiffType != Filemap.FileDiff.Type.Delete);
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
