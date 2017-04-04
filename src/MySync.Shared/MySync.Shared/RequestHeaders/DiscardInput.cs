// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using MySync.Shared.VersionControl;
using Newtonsoft.Json;

namespace MySync.Shared.RequestHeaders
{
    public class DiscardInput
    {
        public ProjectAuthority Authority { get; set; }

        public Filemap.File[] Files { get; set; }

        /// <summary>
        /// Serialze this DiscardInput to json.
        /// </summary>
        /// <returns>The json DiscardInput.</returns>
        public string ToJson()
        {
            // serialize object to JSON
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Create DiscardInput from json string.
        /// </summary>
        /// <param name="json">The json string.</param>
        /// <returns>The built DiscardInput.</returns>
        public static PullInput FromJson(string json)
        {
            // deserialize object from `json` string.
            return JsonConvert.DeserializeObject<PullInput>(json);
        }
    }
}
