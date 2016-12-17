// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MySync.Client.Utilities;

namespace MySync.Client.Core.Projects
{
    /// <summary>
    /// Project class.
    /// </summary>
    public class Project
    {
        public string Name { get; }

        public string RemoteDirectory { get; set; }

        public string LocalDirectory { get; set; }

        public FileSystem FileSystem { get; }

        public Commit Commit { get; set; }

        // hide the constructor
        internal Project(SFtpClient client, string name, string localdir, string remotedir)
        {
            Name = name;
            LocalDirectory = localdir;
            RemoteDirectory = remotedir;

            RemoteDirectory = ClientSettings.Instance.MainDirectory + "/projects/" + Name;

            FileSystem = new FileSystem
            {
                Project = this
            };

            FileSystem.BuildFilemap();
            FileSystem.Open(client);
        }

        public void Lock()
        {
            // lock
            FileSystem.Client.Execute("echo 1 >" + RemoteDirectory + "/lockfile");
        }

        public void Unlock()
        {
            // unlock
            FileSystem.Client.Execute("echo 0 >" + RemoteDirectory + "/lockfile");
        }

        public bool IsUpToDate(out int commitId)
        {
            commitId = 0;
            // TODO: compare latest downloaded commit to the remote commits

            // find remote commit id
            // find local commit id

            var remoteFiles = FileSystem.GetFilesRemote("/commits");
            var localFiles = FileSystem.GetFilesLocal("/commits");

            if (remoteFiles.Length == 0)
                return true;

            var highestRemote = GetHighestCommitId(remoteFiles);
            var highestLocal = GetHighestCommitId(localFiles);

            if(highestLocal > highestRemote)
                throw new Exception("INVALID LOCAL COMMITS!");

            commitId = highestRemote;
            return highestLocal == highestRemote;
        }

        public int GetCurrentCommit()
        {
            var localFiles = FileSystem.GetFilesLocal("/commits");

            return localFiles.Length == 0 ? 0 : GetHighestCommitId(localFiles);
        }

        public int GetHighestCommitId(string[] files)
        {
            var highest = 0;
            foreach (var file in files)
            {
                // TODO: some validation
                highest = Math.Max(highest, GetCommitId(file));
            }

            return highest;
        }

        public int GetCommitId(string file)
        {
            // commit_X.json
            //   |      +-- remove '.json'
            //   +--------- remote 'commit_'
            // this will give only 'X'

            var filename = file.Replace("commit_", "");
            filename = filename.Replace(".json", "");
            return int.Parse(filename);
        }

        public bool IsLocked()
        {
            var data = FileSystem.Client.Execute("cat " + RemoteDirectory + "/lockfile");
            return data[0] == '1';
        }

        public void LockFile(string file)
        {
            // lock file
            // this will be in the future
        }

        public void UnlockFile(string file)
        {
            // unlock file
            // this will be in the future
        }

        public void CreateCommit(string name)
        {
            Commit = new Commit
            {
                CommitDescription = name
            };
        }
        
        public void Discard(Commit.CommitEntry entry)
        {
            switch (entry.EntryType)
            {
                case CommitEntryType.Created:
                    // delete
                    FileSystem.DeleteLocalFile(entry.Entry);
                    break;
                case CommitEntryType.Deleted:
                    // download from server
                    if (!IsLocked())
                    {
                        
                    }
                    break;
                case CommitEntryType.Changed:
                    // download from server
                    if (!IsLocked())
                    {

                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Push(List<Commit.CommitEntry> excluded)
        {
            // TODO: progress
            if (Commit.FileChanges.Count == 0)
            {
                // nothing to push
                return;
            }

            int commitId;
            if (!IsUpToDate(out commitId))
            {
                // show error?
                return;
            }

            // send the current commit
            if (!IsLocked())
            {
                try
                {
                    Lock();
                    
                    // --


                    // Create commit
                    var newCommitid = commitId + 1;

                    // push commit
                    var commitJson = Commit.ToJson();

                    // write remote
                    FileSystem.Client.Upload(commitJson, RemoteDirectory + "/commits/commit_" + newCommitid + ".json");

                    // write local
                    File.WriteAllText(LocalDirectory + "/commits/commit_" + newCommitid + ".json", commitJson);

                    // --


                    // Update filemap
                    var mapping = FileSystem.GetLocalMapping().Exclude(excluded);
                    
                    var filemapJson = mapping.ToJson();
                    FileSystem.Client.Upload(filemapJson, RemoteDirectory + "/filemap");

                    // do remote changes
                    foreach (var entry in Commit.FileChanges)
                    {
                        if (entry.EntryType == CommitEntryType.Deleted)
                        {
                            // delete file
                            FileSystem.Client.DeleteFile(RemoteDirectory + "/data/" + entry.Entry);
                        }
                        else
                        {
                            // upload file
                            var lf = LocalDirectory + "\\data\\" + entry.Entry.Replace("/", "\\");
                            var rf = RemoteDirectory + "/data/" + entry.Entry;
                            
                            FileSystem.Client.UploadFile(lf, rf);

                            // TODO: Optimize transfer size using some sort of binary diff?
                        }
                    }
                    
                    // --

                    
                    // cleanup
                    FileSystem.Client.DeleteEmptyDirs(RemoteDirectory + "/data/");

                    // done
                    Unlock();
                }
                catch
                {
                    // TODO: Show error?
                    Unlock();
                }
            }
        }

        public void Pull()
        {
            // TODO: progress
            // apply all commits
            int commitId;
            if (IsUpToDate(out commitId))
            {
                // show message?
                return;
            }

            if (!IsLocked())
            {
                try
                {
                    Lock();

                    // download commits, start from commit_'commitId'.json
                    var commitFiles = FileSystem.GetFilesRemote("/commits");
                    commitFiles = commitFiles.OrderBy(x => x).ToArray();

                    var cCommit = GetCurrentCommit();

                    if (cCommit == 0)
                        cCommit = 1;
                    
                    var firstIndex = Array.FindIndex(commitFiles, x => x == "commit_" + cCommit + ".json");
                    var lastIndex = GetCommitId(commitFiles[commitFiles.Length-1]);

                    var commits = new List<Commit>();
                    for (var i = firstIndex+1; i < lastIndex+1; i++)
                    {
                        var commitfile = RemoteDirectory + "/commits/" + "commit_" + i + ".json";
                        var commit = FileSystem.Client.DownloadFile(commitfile);
                        commits.Add(Commit.FromJson(commit));
                    }

                    // calculate download list and download
                    if (GetCurrentCommit() == 0)
                    {
                        // download whole project

                        // get all files
                        var files = FileSystem.GetRemoteMapping().Files;

                        // download files
                        foreach (var file in files)
                        {
                            var outputFile = LocalDirectory + "/data/" + file.File;
                            var remoteFile = RemoteDirectory + "/data/" + file.File;

                            // download
                            try
                            {
                                FileSystem.Client.DownloadFile(outputFile, remoteFile);
                            }
                            catch
                            {
                                Directory.CreateDirectory(PathUtils.GetPath(outputFile));
                                FileSystem.Client.DownloadFile(outputFile, remoteFile);
                            }

                            // set file mod time(version base)
                            File.SetLastWriteTime(outputFile, DateTime.FromBinary(file.Version));
                        }
                    }
                    else
                    {
                        // calculate diff
                        var files = FileSystem.GetRemoteMapping().Files;
                        var changes = Commit.MergeChanges(commits.ToArray());

                        var toDownload = changes.GetFilesToDownload();
                        var toRemove = changes.GetFilesToRemove();
                        
                        // download all changed files
                        foreach (var file in toDownload)
                        {
                            var outputFile = LocalDirectory + "/data/" + file;
                            var remoteFile = RemoteDirectory + "/data/" + file;

                            // download
                            try
                            {
                                FileSystem.Client.DownloadFile(outputFile, remoteFile);
                            }
                            catch
                            {
                                Directory.CreateDirectory(PathUtils.GetPath(outputFile));
                                FileSystem.Client.DownloadFile(outputFile, remoteFile);
                            }

                            // set file mod time(version base)
                            var fileEntry = files.FirstOrDefault(x => x.File == file);
                            File.SetLastWriteTime(outputFile, DateTime.FromBinary(fileEntry.Version));
                        }

                        FileSystem.BuildFilemap();
                        
                        // remove all files
                        foreach (var file in toRemove)
                        {
                            var path = LocalDirectory + "/data/" + file;

                            try
                            {
                                File.Delete(path);
                            }
                            catch
                            {
                                // ignore
                            }
                        }
                    }
                    
                    // apply the commits as local commits
                    var cid = cCommit;
                    foreach (var commit in commits)
                    {
                        var json = commit.ToJson();
                        var fileName = "commit_" + cid + ".json";
                        File.WriteAllText(LocalDirectory + "/commits/" + fileName, json);
                        cid++;
                    }
                    
                    // done
                    Unlock();
                }
                catch
                {
                    // TODO: Show error?
                    Unlock();
                }
            }
        }
    }
}