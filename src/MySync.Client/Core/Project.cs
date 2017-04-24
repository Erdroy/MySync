// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using MySync.Shared.RequestHeaders;
using MySync.Shared.VersionControl;
using Newtonsoft.Json.Linq;

namespace MySync.Client.Core
{
    /// <summary>
    /// Project class.
    /// </summary>
    public class Project : IDisposable
    {
        private Filemap _lastFilemap;
        private Filemap _currentFilemap;

        /// <summary>
        /// Refresh project changes.
        /// </summary>
        public void Refresh()
        {
            // load last filemap
            var filemapData = File.ReadAllText(RootDir + ".mysync\\last_filemap.json");
            _lastFilemap = Filemap.FromJson(filemapData);

            // build current filemap
            _currentFilemap = Filemap.Build(RootDir);
        }

        /// <summary>
        /// Build diff list.
        /// </summary>
        /// <returns>The diff list.</returns>
        public Filemap.FileDiff[] BuildDiff()
        {
            return _lastFilemap.GetDiff(_currentFilemap);
        }

        /// <summary>
        /// Gets the last filemap.
        /// </summary>
        /// <returns>The last commit base filemap.</returns>
        public Filemap GetLastFilemap()
        {
            return _lastFilemap;
        }

        /// <summary>
        /// Discard selected files.
        /// </summary>
        /// <param name="files">Files selected for discard.</param>
        public void Discard(Filemap.FileDiff[] files)
        {
            throw new Exception("Discard method is not implemented, yet.");

            // select all files that are not 'created'
            var filesToDownload = new List<Filemap.File>();

            foreach (var file in files)
            {
                if (file.DiffType != Filemap.FileDiff.Type.Created)
                {
                    filesToDownload.Add(new Filemap.File
                    {
                        FileName = file.FileName
                    });
                }
            }

            var input = new DiscardInput
            {
                Authority = Authority,
                Files = filesToDownload.ToArray()
            };

            // remove new files
            var toDelete = 0;
            foreach (var file in files)
            {
                if (file.DiffType == Filemap.FileDiff.Type.Created) // delete all 'created' files
                {
                    toDelete++;
                    // TODO: delete file
                }
            }
            
            // check if there are files only for delete
            if (toDelete == files.Length)
            {
                return;
            }

            // download original files from server
            Request.Send(ServerAddress + "discard", input.ToJson(), stream =>
            {
                using (var reader = new BinaryReader(stream))
                {
                    // TODO: read commit

                    // download
                    var dataFile = RootDir + ".mysync/commit_recv.zip";
                    using (var fs = File.Create(dataFile))
                    {
                        var fileLength = reader.ReadInt64();

                        int read;
                        var buffer = new byte[64 * 1024];
                        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fs.Write(buffer, 0, read);
                        }
                    }

                    // TODO: apply commit
                }
            });
        }

        /// <summary>
        /// Push commit to the server.
        /// </summary>
        /// <param name="commit">The commit.</param>
        /// <param name="dataFile">The commit data file.</param>
        /// <param name="onProgress">This is called when there is some progress made.</param>
        public void Push(Commit commit, string dataFile, Action<int> onProgress)
        {
            // construct pull input data
            var dataJson = new PullInput
            {
                Authority = Authority
            };
            var lastCommitId = -1;
            Request.Send(ServerAddress + "getcommit", dataJson.ToJson(), stream =>
            {
                using (var reader = new BinaryReader(stream))
                {
                    var message = reader.ReadString();

                    if (message == "Done")
                    {
                        lastCommitId = reader.ReadInt32();
                    }
                    else
                    {
                        throw new Exception("Cannot push, Error: <br>" + message);
                    }
                }
            });

            // select current commit id
            var commitInfo = RootDir + ".mysync/commit_info.txt";
            var currentCommitId = File.Exists(commitInfo) ? int.Parse(File.ReadAllText(commitInfo)) : -1;

            if (lastCommitId > currentCommitId)
            {
                throw new Exception("Cannot push, project is not up-to-date!");
            }

            var clientData = Encoding.UTF8.GetBytes(Authority.ToJson());
            var commitData = Encoding.UTF8.GetBytes(commit.ToJson());

            var filemap = _lastFilemap;
            filemap.AddChanges(RootDir, commit.Files);
            var filemapJson = filemap.ToJson();

            using (var file = new FileStream(dataFile, FileMode.Open))
            {
                var fileNeeded = commit.IsUploadNeeded();
                var datasize = (fileNeeded ? file.Length : 0) + clientData.Length + commitData.Length + 2 * sizeof(int) + sizeof(bool); // <---

                // begin send
                var stream = Request.BeginSend(ServerAddress + "push", datasize);

                using (var writer = new BinaryWriter(stream))
                {
                    // write client data header
                    writer.Write(clientData.Length);
                    writer.Write(clientData);

                    // write data header
                    writer.Write(commitData.Length);
                    writer.Write(commitData);

                    writer.Write(fileNeeded);

                    if (fileNeeded)
                    {
                        // upload commit data file
                        var readbytes = 0;
                        var totalbytes = (int)file.Length;
                        int read;
                        var buffer = new byte[64 * 1024];
                        while ((read = file.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            writer.Write(buffer, 0, read);
                            
                            readbytes += read;

                            var prc = (float)readbytes / totalbytes;
                            prc *= 100.0f;
                            onProgress((int)prc);
                        }
                    }

                    onProgress(100);

                    Request.EndSend(resp =>
                    {
                        // done!
                        // read response data
                        using (var reader = new BinaryReader(resp))
                        {
                            var message = reader.ReadString();

                            // finalize everything
                            if (message.StartsWith("#RESTORE"))
                            {
                                throw new Exception("Commit failed, error: " + message);
                            }

                            if (message.StartsWith("Failed"))
                            {
                                throw new Exception("Commit failed, error: " + message);
                            }

                            var commitId = reader.ReadInt32();

                            // save commit id
                            File.WriteAllText(commitInfo, commitId.ToString());

                            // save filemap
                            File.WriteAllText(RootDir + ".mysync/last_filemap.json", filemapJson);
                            
                            // refresh
                            Refresh();
                        }
                    });
                }
            }

            // delete data file
            File.Delete(dataFile);

            // ok
        }

        /// <summary>
        /// Pull commits from server and apply.
        /// </summary>
        /// <param name="onProgress">This is called when there is some progress made.</param>
        public void Pull(Action<int> onProgress)
        {
            var commitInfo = RootDir + ".mysync/commit_info.txt";

            // select current commit id
            var currentCommitId = File.Exists(commitInfo) ? int.Parse(File.ReadAllText(commitInfo)) : -1;

            // construct pull input data
            var dataJson = new PullInput
            {
                Authority = Authority,
                CommitId = currentCommitId
            };
            
            // send pull request
            Request.Send(ServerAddress + "pull", dataJson.ToJson(), stream =>
            {
                using (var reader = new BinaryReader(stream))
                {
                    var body = reader.ReadString();

                    Console.WriteLine(body);

                    Commit commit; // try convert commit data
                    try
                    {
                        commit = Commit.FromJson(body);
                    }
                    catch
                    {
                        throw new WarningException("There is no any changes to download.");
                    }

                    var commitId = reader.ReadInt32();
                    Console.WriteLine(@"commit id: " + commitId);

                    var hasFile = reader.ReadBoolean();

                    var dataFile = RootDir + ".mysync/commit_recv.zip";

                    if (hasFile)
                    {
                        using (var fs = File.Create(dataFile))
                        {
                            var totalbytes = reader.ReadInt64();

                            var readbytes = 0;
                            int read;
                            var buffer = new byte[64*1024];
                            while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                fs.Write(buffer, 0, read);

                                readbytes += read;
                                var prc = (float)readbytes / totalbytes;
                                prc *= 100.0f;
                                onProgress((int)prc);
                            }
                        }
                    }

                    // apply the commit
                    commit.Apply(RootDir, dataFile, hasFile);

                    // remove data file
                    if(hasFile)
                        File.Delete(dataFile);

                    // update filemap
                    _lastFilemap.AddChanges(RootDir, commit.Files);
                    File.WriteAllText(RootDir + ".mysync/last_filemap.json", _lastFilemap.ToJson());

                    // save commit id
                    File.WriteAllText(commitInfo, commitId.ToString());

                    // refresh
                    Refresh();
                }
            });
        }

        /// <summary>
        /// Dispose the project.
        /// </summary>
        public void Dispose()
        {
            // ?
        }

        /// <summary>
        /// Create new project.
        /// </summary>
        /// <param name="directory">The directory for the project, eg.: 'D:\\SomeProject'</param>
        /// <param name="name">The project name.</param>
        /// <returns>The created project, or null when can't create project in this directory.</returns>
        public static Project Create(string directory, string name)
        {
            if (!directory.EndsWith("\\"))
                directory += "\\";

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var dir = Directory.GetFiles(directory, "*.*");
            
            // check if this directory is empty
            if (dir.Length == 0)
            {
                var project = new Project
                {
                    RootDir = directory
                };

                var mysyncDataDir = directory + ".mysync";
                var msDir = Directory.CreateDirectory(mysyncDataDir);

                // hide the directory
                msDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

                // build empty for 'last commit'.
                var emptyFilemap = Filemap.BuildEmpty();
                project._lastFilemap = emptyFilemap;

                // save
                File.WriteAllText(mysyncDataDir + "\\last_filemap.json", emptyFilemap.ToJson());

                // refresh the changes
                project.Refresh();

                // return created project
                return project;
            }

            return null; // nope!
        }

        /// <summary>
        /// Open project from directory.
        /// </summary>
        /// <param name="address">The server address.</param>
        /// <param name="name">The project name.</param>
        /// <param name="directory">The project directory.</param>
        /// <returns>The opened project or null when not found.</returns>
        public static Project OpenWorkingCopy(string address, string name, string directory)
        {
            if (!directory.EndsWith("\\"))
                directory += "\\";

            var mysyncDataDir = directory + ".mysync";

            // check if this is mysync-project directory.
            if (!Directory.Exists(mysyncDataDir))
                return null; // nope! can't open project

            var project = new Project
            {
                RootDir = directory,
                ServerAddress = address,
                ProjectName = name
            };
            project.Refresh();

            return project;
        }

        /// <summary>
        /// Open remote project.
        /// </summary>
        /// <param name="address">The server address.</param>
        /// <param name="name">The project name.</param>
        /// <param name="username">Username that will be used to download the project.</param>
        /// <param name="accesstoken">Password that will be used to download the project.</param>
        /// <param name="directory">The directory when the project will be downloaded.</param>
        /// <returns>The project, null when something failed.</returns>
        public static Project OpenProject(string address, string name, string username, string accesstoken, string directory)
        {
            var authority = new ProjectAuthority
            {
                ProjectName = name,
                Username = username,
                AccessToken = accesstoken
            };

            // send project authority request
            var response = Request.Send(address + "authorize", authority.ToJson());
            string responseBody;
            using (var sr = new BinaryReader(response))
            {
                responseBody = sr.ReadString();
            }

            var authResult = JObject.Parse(responseBody);
            var authorized = authResult["auth"].ToObject<bool>();

            if (!authorized)
                return null;

            // setup directory
            if (!directory.EndsWith("\\"))
                directory += "\\";

            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var dir = Directory.GetFiles(directory, "*.*");

            if (dir.Length > 0) // directory is not empty!
                return null;
            
            // create local project
            var project = new Project
            {
                RootDir = directory,
                ServerAddress = address,
                Authority = authority
            };

            var mysyncDataDir = directory + ".mysync";
            var msDir = Directory.CreateDirectory(mysyncDataDir);

            // hide the directory
            msDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            // build empty for 'last commit'.
            var emptyFilemap = Filemap.BuildEmpty();
            project._lastFilemap = emptyFilemap;

            // save
            File.WriteAllText(mysyncDataDir + "\\last_filemap.json", emptyFilemap.ToJson());

            // refresh the changes
            project.Refresh();

            // download all files
            project.Pull(x => { }); // TODO: progress
            
            return project;
        }
        
        /*public static void CreateProject(string address, string name, string directory)
        {
            if (!directory.EndsWith("\\"))
                directory += "\\";

            if (Directory.Exists(directory))
                return; // nope! can't create project


            // send request
            // create local project
        }*/

        /// <summary>
        /// Project authority,
        /// contains all information about the authority of this project,
        /// for current user.
        /// </summary>
        public ProjectAuthority Authority { get; set; }

        /// <summary>
        /// The project server address.
        /// </summary>
        public string ServerAddress { get; set; }

        /// <summary>
        /// The project name.
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// The local project directory.
        /// </summary>
        public string RootDir { get; set; }
    }
}