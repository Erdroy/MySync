// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

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

        public bool IsUpToDate()
        {
            return true;
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

        public void AddChanges(string file)
        {
            // add file to commit
        }

        public void RemoveChanges(string file)
        {
            // remove file from commit
        }

        public void Discard(string file)
        {
            // download the file from the server
        }

        public void Push(string message)
        {
            // send the current commit
        }

        public void Pull()
        {

        }
    }
}