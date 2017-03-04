// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using Newtonsoft.Json;

namespace MySync.Shared.VersionControl
{
    /// <summary>
    /// Commit class - holds update data.
    /// </summary>
    public class Commit
    {
        /// <summary>
        /// The commit description.
        /// Default: 'No message' #gitlike
        /// </summary>
        public string Description = "No message";

        /// <summary>
        /// All files in this commit.
        /// </summary>
        public Filemap.FileDiff[] Files;
        
        /// <summary>
        /// Serialze this Commit to json.
        /// </summary>
        /// <returns>The json Commit.</returns>
        public string ToJson()
        {
            // serialize object to JSON
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Create commit from diff.
        /// </summary>
        /// <param name="diff">The files diff.</param>
        /// <param name="desc">(optional)The commit description.</param>
        /// <returns>The created commit.</returns>
        public static Commit FromDiff(Filemap.FileDiff[] diff, string desc = "No message")
        {
            var commit = new Commit
            {
                Files = diff
            };
            return commit;
        }

        /// <summary>
        /// Create Commit from json string.
        /// </summary>
        /// <param name="json">The json string.</param>
        /// <returns>The built Commit.</returns>
        public static Commit FromJson(string json)
        {
            // deserialize object from `json` string.
            return JsonConvert.DeserializeObject<Commit>(json);
        }
    }
}
