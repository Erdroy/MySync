// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MySync.Server.Core.RequestHandlers
{
    /// <summary>
    /// Authorize request handler.
    /// </summary>
    public class Authorize
    {
        public struct AuthorizeOutput
        {
            public bool Succeeded;
            public string AccessToken;
            public string[] Projects;
        }

        /// <summary>
        /// Processor-method
        /// </summary>
        public static void Process(string body, HttpListenerResponse response)
        {
            var json = JObject.Parse(body);

            var username = json["username"];
            var password = json["password"];

            // TODO: check user

            var output = new AuthorizeOutput
            {
                Succeeded = true,
                AccessToken = "ANONYMOUS_TOKEN",
                Projects = new []
                {
                    "test"
                }
            };

            // send output
            var outputData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(output));
            response.OutputStream.Write(outputData, 0, outputData.Length);
        }
    }
}
