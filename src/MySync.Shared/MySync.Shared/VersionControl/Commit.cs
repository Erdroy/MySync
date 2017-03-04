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
