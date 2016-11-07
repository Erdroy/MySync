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
        internal Project(SFtpClient client, string name)
        {
            Name = name;

            RemoteDirectory = ClientSettings.Instance.MainDirectory + "/projects/" + Name;

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
            return data[0] == '1';
        }
    }
}