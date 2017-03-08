// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using MySync.Shared.VersionControl;

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
        
        /// <summary>
        /// Convert commit model to commit structure.
        /// </summary>
        /// <returns>The created commit structure.</returns>
        public Commit ToCommit()
        {
            var commit = new Commit
            {
                Description = CommitDescription,
                Files = new Filemap.FileDiff[Files.Length]
            };

            for (var i = 0; i < Files.Length; i++)
            {
                commit.Files[i] = new Filemap.FileDiff
                {
                    FileName = Files[i].Name,
                    DiffType = (Filemap.FileDiff.Type)Files[i].Operation,
                    Version = Files[i].Version
                };
            }

            return commit;
        }
    }
}
