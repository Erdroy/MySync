// MySync © 2016-2017 Damian 'Erdroy' Korczowski

namespace MySync.Server.Core.DatabaseModels
{
    /// <summary>
    /// Commit structure model for database.
    /// </summary>
    public class CommitModel
    {
        public struct FileDiff
        {
            public string Name { get; set; }

            public int Operation { get; set; }

            public long Version { get; set; }
        }

        public int CommitId { get; set; }

        public string CommitDescription { get; set; }

        public FileDiff[] Files { get; set; }
    }
}
