// MySync © 2016 Damian 'Erdroy' Korczowski


using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MySync.Client.Core
{
    public class Commit
    {
        public struct CommitEntry
        {
            public string Entry;
            public CommitEntryType EntryType;

            public CommitEntry(CommitEntryType entryType, string entry)
            {
                Entry = entry;
                EntryType = entryType;
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

        public string[] GetFilesToDownload()
        {
            return (from change in FileChanges where change.EntryType != CommitEntryType.Deleted select change.Entry).ToArray();
        }

        public string[] GetFilesToRemove()
        {
            return (from change in FileChanges where change.EntryType == CommitEntryType.Deleted select change.Entry).ToArray();
        }

        public static Commit MergeChanges(Commit[] commits)
        {
            var output = new Commit();

            foreach (var commit in commits)
            {
                foreach (var change in commit.FileChanges)
                {
                    if (output.FileChanges.Any(x => x.Entry == change.Entry))
                        output.FileChanges.RemoveAt(output.FileChanges.FindIndex(x => x.Entry == change.Entry));

                    if (change.EntryType == CommitEntryType.Deleted)
                    {
                        output.FileChanges.Add(new CommitEntry
                        {
                            Entry = change.Entry,
                            EntryType = CommitEntryType.Deleted
                        });
                    }
                    else
                    {
                        output.FileChanges.Add(new CommitEntry
                        {
                            Entry = change.Entry,
                            EntryType = CommitEntryType.Created
                        });
                    }
                }
            }

            return output;
        }
        
        public static Commit FromJson(string jsonsource)
        {
            return JsonConvert.DeserializeObject<Commit>(jsonsource);
        }
        
        public string CommitDescription { get; set; }

        public List<CommitEntry> FileChanges { get; }
    }
}