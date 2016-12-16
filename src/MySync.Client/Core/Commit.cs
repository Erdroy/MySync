// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System.Collections.Generic;
using Newtonsoft.Json;

namespace MySync.Client.Core
{
    public class Commit
    {
        public struct CommitEntry
        {
            public CommitEntryType EntryType;
            public string Entry;

            public CommitEntry(CommitEntryType entryType, string entry)
            {
                EntryType = entryType;
                Entry = entry;
            }
        }

        public Commit()
        {
            FileChanges = new List<CommitEntry>();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        public static string[] CalculateDownloadable(Commit[] commits)
        {
            var toDownload = new List<string>();

            foreach (var commit in commits)
            {
                foreach (var change in commit.FileChanges)
                {
                    if (change.EntryType == CommitEntryType.Deleted)
                    {
                        if (toDownload.Contains(change.Entry))
                            toDownload.Remove(change.Entry);
                    }
                    else
                    {
                        if(!toDownload.Contains(change.Entry))
                            toDownload.Add(change.Entry);
                    }
                }
            }

            return toDownload.ToArray();
        }

        public static Commit FromJson(string jsonsource)
        {
            return JsonConvert.DeserializeObject<Commit>(jsonsource);
        }
        
        public string CommitDescription { get; set; }

        public List<CommitEntry> FileChanges { get; private set; }
    }
}