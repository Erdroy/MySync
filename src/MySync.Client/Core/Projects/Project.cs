// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MySync.Client.UI;
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

        public string[] Exclusions { get; set; }

        // hide the constructor
        internal Project(SFtpClient client, string name, string localdir, string remotedir)
        {
            Name = name;
            LocalDirectory = localdir;
            RemoteDirectory = remotedir;

            RemoteDirectory = "/home/mysync/projects/" + Name;

            FileSystem = new FileSystem
            {
                Project = this
            };

            // load local exclusions
            LoadExclusions();

            FileSystem.BuildFilemap();
            FileSystem.Open(client);
        }

        public void Lock()
        {
            // lock
            FileSystem.Client.Execute("touch " + RemoteDirectory + "/lockfile");
        }

        public void Unlock()
        {
            // unlock
            FileSystem.Client.Execute("rm " + RemoteDirectory + "/lockfile");
        }

        public bool IsLocked()
        {
            const string lockfile = "lockfile";

            var data = FileSystem.Client.Execute("ls " + RemoteDirectory + " |grep " + lockfile);
            return data.Length >= lockfile.Length;
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

        public void LoadExclusions()
        {
            var file = LocalDirectory + "/data/.ignore";
            if (File.Exists(file))
            {
                var data = File.ReadAllText(file);
                data = data.Replace("\r", "").Trim();
                Exclusions = data.Split('\n');
            }
        }
        
        public void Discard(Commit.CommitEntry entry)
        {
            switch (entry.EntryType)
            {
                case CommitEntryType.Created:
                    // delete
                    FileSystem.DeleteLocalFile(LocalDirectory + "/data/" + entry.Entry);
                    break;
                case CommitEntryType.Changed:
                case CommitEntryType.Deleted:
                    // download from server
                    if (!IsLocked())
                    {
                        var files = FileSystem.GetRemoteMapping().Files;
                        using (new ProjectLock(this))
                        {
                            var outputFile = LocalDirectory + "/data/" + entry.Entry;
                            var remoteFile = RemoteDirectory + "/data/" + entry.Entry;

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
                            var fileEntry = files.FirstOrDefault(x => x.File == entry.Entry);
                            File.SetLastWriteTime(outputFile, DateTime.FromBinary(fileEntry.Version));
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            FileSystem.DeleteEmptyDirs(LocalDirectory + "/data/");
        }

        public void Push(List<Commit.CommitEntry> excluded)
        {
            TaskManager.DispathSingle(delegate
            {
                Progress.Message = "Checking for updates...";
            });

            // TODO: progress
            if (Commit.FileChanges.Count == 0)
            {
                // nothing to push
                TaskManager.DispathSingle(delegate
                {
                    Message.ShowMessage("", "There is no staged changes.");
                });
                return;
            }

            int commitId;
            if (!IsUpToDate(out commitId))
            {
                // show error?
                TaskManager.DispathSingle(delegate
                {
                    Message.ShowMessage("Warning", "Cannot push changes, project is not up-to-date! Please pull all changes before pushing.");
                });
                return;
            }

            // send the current commit
            if (!IsLocked())
            {
                try
                {
                    using (new ProjectLock(this))
                    {
                        // --
                        TaskManager.DispathSingle(delegate
                        {
                            Progress.Message = "Uploading commit...";
                        });
                        // Create commit
                        var newCommitid = commitId + 1;

                        // push commit
                        var commitJson = Commit.ToJson();

                        // write remote
                        FileSystem.Client.Upload(commitJson,
                            RemoteDirectory + "/commits/commit_" + newCommitid + ".json");

                        // write local
                        File.WriteAllText(LocalDirectory + "/commits/commit_" + newCommitid + ".json", commitJson);

                        // --


                        // upload filemap
                        var mapping = FileSystem.GetLocalMapping().Exclude(excluded);

                        var filemapJson = mapping.ToJson();
                        FileSystem.Client.Upload(filemapJson,
                            RemoteDirectory + "/filemaps/filemap_" + newCommitid + ".json");

                        // do remote changes
                        var fileId = 0;
                        var files = Commit.FileChanges.Count;
                        foreach (var entry in Commit.FileChanges)
                        {
                            var id = fileId;
                            
                            TaskManager.DispathSingle(delegate
                            {
                                Progress.Message = "Updating file " + id + " out of " + files;
                            });
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

                                rf = PathUtils.Encode(rf);

                                FileSystem.Client.UploadFile(lf, rf);

                                // TODO: Optimize transfer size using some sort of binary diff?
                            }

                            fileId++;
                        }


                        // --

                        TaskManager.DispathSingle(delegate
                        {
                            Progress.Message = "Cleaning...";
                        });

                        // cleanup
                        FileSystem.Client.DeleteEmptyDirs(RemoteDirectory + "/data/");
                    }

                    TaskManager.DispathSingle(delegate
                    {
                        LoadExclusions();
                        Message.ShowMessage("", "Pushed all changes!");
                    });
                }
                catch(Exception ex)
                {
                    Unlock();
                    TaskManager.DispathSingle(delegate
                    {
                        Message.ShowMessage("Error", "Error: " + ex);
                    });
                }
            }
            else
            {
                TaskManager.DispathSingle(delegate
                {
                    Message.ShowMessage("Error", "Sorry, project is currently locked due to PUSH or PULL of other user. If you are sure that no one is using the project, this may be a bug, delete `lockfile` in the remote project directory.");
                });
            }
            FileSystem.DeleteEmptyDirs(LocalDirectory + "/data/");
        }

        public void Pull()
        {
            // apply all commits
            int commitId;
            if (IsUpToDate(out commitId))
            {
                // show message?
                TaskManager.DispathSingle(delegate
                {
                    Message.ShowMessage("", "No changes to download.");
                });
                return;
            }

            if (!IsLocked())
            {
                try
                {
                    using (new ProjectLock(this))
                    {
                        FileSystem.IgnoreChanges = true;

                        TaskManager.DispathSingle(delegate
                        {
                            Progress.Message = "Checking for updates...";
                        });
                        // download commits, start from commit_'commitId'.json
                        var commitFiles = FileSystem.GetFilesRemote("/commits");
                        commitFiles = commitFiles.OrderBy(x => x).ToArray();

                        var cCommit = GetCurrentCommit();

                        if (cCommit == 0)
                            cCommit = 1;

                        var firstIndex = Array.FindIndex(commitFiles, x => x == "commit_" + cCommit + ".json");
                        var lastIndex = GetCommitId(commitFiles[commitFiles.Length - 1]);

                        var commits = new List<Commit>();
                        for (var i = firstIndex + 2; i < lastIndex + 1; i++)
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
                            var files = FileSystem.GetRemoteMapping(lastIndex).Files;

                            // download files
                            var filesDownloaded = 0;
                            foreach (var file in files)
                            {
                                var downloaded = filesDownloaded;
                                TaskManager.DispathSingle(delegate
                                {
                                    Progress.Message = "Downlading file " + (downloaded+1) + " out of " + files.Count;
                                });

                                var fileName = file.File.Replace("%20", " ");

                                var outputFile = LocalDirectory + "/data/" + fileName;
                                var remoteFile = RemoteDirectory + "/data/" + file.File;
                                
                                remoteFile = PathUtils.Encode(remoteFile);

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

                                if (fileName == ".ignore")
                                    LoadExclusions();

                                var id = 1;
                                foreach (var commit in commits)
                                {
                                    var json = commit.ToJson();
                                    File.WriteAllText(LocalDirectory + "/commits/" + "commit_" + id + ".json", json);
                                    id++;
                                }

                                // set file mod time(version base)
                                File.SetLastWriteTime(outputFile, DateTime.FromBinary(file.Version));
                                filesDownloaded++;
                            }
                        }
                        else
                        {
                            TaskManager.DispathSingle(delegate
                            {
                                Progress.Message = "Saving changes...";
                            });
                            // apply the commits as local commits
                            var cid = cCommit + 1;
                            foreach (var commit in commits)
                            {
                                var json = commit.ToJson();
                                var fileName = "commit_" + cid + ".json";
                                File.WriteAllText(LocalDirectory + "/commits/" + fileName, json);
                                cid++;
                            }

                            // calculate diff
                            var files = FileSystem.GetRemoteMapping().Files;
                            var changes = Commit.MergeChanges(commits.ToArray());

                            var toDownload = changes.GetFilesToDownload();
                            var toRemove = changes.GetFilesToRemove();

                            // download all changed files
                            var filesDownloaded = 0;
                            foreach (var file in toDownload)
                            {
                                var downloaded = filesDownloaded;
                                TaskManager.DispathSingle(delegate
                                {
                                    Progress.Message = "Downlading file " + (downloaded + 1) + " out of " + files.Count;
                                });

                                var outputFile = LocalDirectory + "/data/" + file;
                                var remoteFile = RemoteDirectory + "/data/" + file;

                                remoteFile = PathUtils.Encode(remoteFile);

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
                                filesDownloaded++;
                            }

                            TaskManager.DispathSingle(delegate
                            {
                                Progress.Message = "Finishing...";
                                LoadExclusions();
                            });

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
                                    FileSystem.IgnoreChanges = false;
                                    // ignore
                                }
                            }
                        }
                    }
                    
                }
                catch(Exception ex)
                {
                    Unlock();
                    Message.ShowMessage("Error", "Error: " + ex);
                }
            }

            FileSystem.Changed = true;
            FileSystem.BuildFilemap();

            FileSystem.IgnoreChanges = false;
            TaskManager.DispathSingle(delegate
            {
                Progress.Message = "Done!";
                Message.ShowMessage("", "Done!");
            });
            FileSystem.DeleteEmptyDirs(LocalDirectory + "/data/");
        }
    }
}