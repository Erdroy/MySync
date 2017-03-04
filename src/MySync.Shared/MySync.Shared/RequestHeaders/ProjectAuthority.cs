// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using Newtonsoft.Json;

namespace MySync.Shared.RequestHeaders
{
    /// <summary>
    /// ProjectAuthority header.
    /// </summary>
    public class ProjectAuthority
    {
        public string ProjectName { get; set; }
        public string Username { get; set; }
        public string AccessToken { get; set; }
        
        /// <summary>
        /// Serialze this ProjectAuthority to json.
        /// </summary>
        /// <returns>The json ProjectAuthority.</returns>
        public string ToJson()
        {
            // serialize object to JSON
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        
        /// <summary>
        /// Create ProjectAuthority from json string.
        /// </summary>
        /// <param name="json">The json string.</param>
        /// <returns>The built ProjectAuthority.</returns>
        public static ProjectAuthority FromJson(string json)
        {
            // deserialize object from `json` string.
            return JsonConvert.DeserializeObject<ProjectAuthority>(json);
        }
    }
}
