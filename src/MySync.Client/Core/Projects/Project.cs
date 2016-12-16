// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System;
using System.Collections.Generic;
using System.IO;
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

        public int GetHighestCommitId(string[] files)
        {
            var highest = 0;
            foreach (var file in files)
            {
                // commit_X.json
                //   |      +-- remove '.json'
                //   +--------- remote 'commit_'
                // this will give only 'X'

                var filename = file.Replace("commit_", "");
                filename = filename.Replace(".json", "");

                // TODO: some validation
                highest = Math.Max(highest, int.Parse(filename));
            }

            return highest;
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
                    // TODO: Throw error?
                    Unlock();
                }
            }
        }

        public void Pull()
        {
            // apply all commits

        }
    }
}