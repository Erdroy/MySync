// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.IO;
using System.Text;
using MySync.Shared.RequestHeaders;
using MySync.Shared.VersionControl;

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
        /// Push commit to the server.
        /// </summary>
        /// <param name="commit">The commit.</param>
        /// <param name="dataFile">The commit data file.</param>
        public void Push(Commit commit, string dataFile)
        {
            // TODO: check if data file exists
            
            var clientData = Encoding.UTF8.GetBytes(Authority.ToJson());
            var commitData = Encoding.UTF8.GetBytes(commit.ToJson());

            var filemap = _lastFilemap;
            filemap.AddChanges(RootDir, commit.Files);
            var filemapJson = filemap.ToJson();

            using (var file = new FileStream(dataFile, FileMode.Open))
            {
                var datasize = file.Length + clientData.Length + commitData.Length + 2 * sizeof(int); // <---

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
                    
                    // upload commit data file
                    int read;
                    var buffer = new byte[64 * 1024];
                    while ((read = file.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, read);
                    }
                    Console.WriteLine(file.Length);

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
                            Console.WriteLine(@"Commit failed error: " + message);
                            return;
                        }

                        var commitId = reader.ReadInt32();

                        // save commit id
                        var commitInfo = RootDir + ".mysync/commit_info.txt";
                        File.WriteAllText(commitInfo, commitId.ToString());

                        // save filemap
                        File.WriteAllText(RootDir + ".mysync/last_filemap.json", filemapJson);

                        // show info
                        Console.WriteLine(message + @" commmitid: " + commitId);

                        // refresh
                        Refresh();
                    }
                });
                }
            }

            // delete data file
            File.Delete(dataFile);
        }

        /// <summary>
        /// Pull commits from server and apply.
        /// </summary>
        public void Pull()
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
                        return;
                    }

                    var commitId = reader.ReadInt32();
                    Console.WriteLine(@"commit id: " + commitId);

                    var dataFile = RootDir + ".mysync/commit_recv.zip";
                    using (var fs = File.Create(dataFile))
                    {
                        int read;
                        var buffer = new byte[64 * 1024];
                        while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fs.Write(buffer, 0, read);
                        }
                    }
                    
                    // apply the commit
                    commit.Apply(RootDir, dataFile);
                    
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
        /// <param name="directory">The project directory.</param>
        /// <returns>The opened project or null when not found.</returns>
        public static Project Open(string address, string directory)
        {
            if (!directory.EndsWith("\\"))
                directory += "\\";

            var mysyncDataDir = directory + ".mysync";

            // check if this is mysync-project directory.
            if (Directory.Exists(mysyncDataDir))
            {
                var project = new Project
                {
                    RootDir = directory,
                    ServerAddress = address
                };
                project.Refresh();

                return project;
            }
            
            // nope! can't open project
            return null;
        }


        public ProjectAuthority Authority { get; set; }

        public string ServerAddress { get; set; }

        public string RootDir { get; set; }
    }
}