
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

        public static Commit FromJson(string jsonsource)
        {
            return JsonConvert.DeserializeObject<Commit>(jsonsource);
        }

        public string CommitDescription { get; set; }

        public List<CommitEntry> FileChanges { get; private set; }
    }
}