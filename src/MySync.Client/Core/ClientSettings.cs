// MySync © 2016-2017 Damian 'Erdroy' Korczowski

namespace MySync.Client.Core
{
    /// <summary>
    /// ClientSettings class.
    /// </summary>
    public class ClientSettings
    {
        public class ProjectSettings
        {
            public string Address { get; set; }

            public string Name { get; set; }

            public string RootDir { get; set; }
        }

        public ProjectSettings[] Projects { get; set; }
    }
}
