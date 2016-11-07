// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

namespace MySync.Client.Core.Projects
{
    /// <summary>
    /// Project class.
    /// </summary>
    public class Project
    {
        public string Name { get; private set; }

        public string RemoteDirectory { get; set; }

        public string LocalDirectory { get; set; }

        public FileSystem FileSystem { get; }

        // hide the constructor
        internal Project(SFtpClient client, string name)
        {
            Name = name;
            FileSystem = new FileSystem();

            FileSystem.Open(client);
        }

        public bool IsUpToDate()
        {
            return true;
        }

        public bool IsLocked()
        {
            var data = FileSystem.Client.Execute("cat " + RemoteDirectory + "/lockfile");

            return false;
        }
    }
}