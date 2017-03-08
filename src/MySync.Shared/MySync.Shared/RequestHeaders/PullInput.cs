// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using Newtonsoft.Json;

namespace MySync.Shared.RequestHeaders
{
    public class PullInput
    {
        public ProjectAuthority Authority { get; set; }

        public int CommitId;

        /// <summary>
        /// Serialze this PullInput to json.
        /// </summary>
        /// <returns>The json PullInput.</returns>
        public string ToJson()
        {
            // serialize object to JSON
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Create PullInput from json string.
        /// </summary>
        /// <param name="json">The json string.</param>
        /// <returns>The built PullInput.</returns>
        public static PullInput FromJson(string json)
        {
            // deserialize object from `json` string.
            return JsonConvert.DeserializeObject<PullInput>(json);
        }
    }
}
