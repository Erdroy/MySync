// MySync © 2016-2017 Damian 'Erdroy' Korczowski

namespace MySync.Client.Core
{
    public class ProjectsSettings
    {
        public class Project
        {
            public string Name { get; set; }

            public string[] AccessTokens { get; set; }
        }

        public Project[] Projects { get; set; }
    }
}
