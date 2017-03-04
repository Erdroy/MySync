// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.IO;
using System.Text;
using Ionic.Zip;
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
        /// Build commit data file.
        /// </summary>
        /// <param name="commit">The commit which will get it's data file.</param>
        /// <returns>The data file path.</returns>
        public string BuildCommit(Commit commit)
        {
            var dir = RootDir;
            var tempFile = dir + ".mysync\\commit.zip";

            // delete the file when exists.
            if (File.Exists(tempFile))
                File.Delete(tempFile);

            // compress all files
            using (var zip = new ZipFile(tempFile))
            {
                foreach (var file in commit.Files)
                {
                    var entry = zip.AddFile(dir + file.FileName); // add file

                    // change name
                    entry.FileName = file.FileName;
                }

                // save
                zip.Save();
            }

            return tempFile;
        }

        /// <summary>
        /// Push commit to the server.
        /// </summary>
        /// <param name="commit">The commit.</param>
        /// <param name="dataFile">The commit data file.</param>
        public void PushCommit(Commit commit, string dataFile)
        {
            // TODO: check if data file exists
            
            var clientData = Encoding.UTF8.GetBytes(Authority.ToJson());
            var commitData = Encoding.UTF8.GetBytes(commit.ToJson());

            using (var file = new FileStream(dataFile, FileMode.Open))
            {
                var datasize = file.Length + commitData.Length + clientData.Length + 2 * sizeof(int);

                // begin send
                var stream = Request.BeginSend("http://127.0.0.1:8080/push", datasize);

                using (var writer = new BinaryWriter(stream))
                {
                    // write client data header
                    writer.Write(clientData.Length);
                    writer.Write(clientData);

                    // write data header
                    writer.Write(commitData.Length);
                    writer.Write(commitData);

                    // TODO: write filemap

                    // upload commit data file
                    int read;
                    var buffer = new byte[64 * 1024];
                    while ((read = file.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, read);
                    }
                }

                Request.EndSend(resp =>
                {
                    // done!
                    // read response data
                    using (var reader = new BinaryReader(resp))
                    {
                        var message = reader.ReadString();
                        // TODO: finalize
                        Console.WriteLine(message);
                    }
                });
            }

            // delete data file
            File.Delete(dataFile);
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
        /// <param name="directory">The project directory.</param>
        /// <returns>The opened project or null when not found.</returns>
        public static Project Open(string directory)
        {
            if (!directory.EndsWith("\\"))
                directory += "\\";

            var mysyncDataDir = directory + ".mysync";

            // check if this is mysync-project directory.
            if (Directory.Exists(mysyncDataDir))
            {
                var project = new Project
                {
                    RootDir = directory
                };
                project.Refresh();

                return project;
            }
            
            // nope! can't open project
            return null;
        }


        public ProjectAuthority Authority { get; set; }

        public string RootDir { get; set; }
    }
}