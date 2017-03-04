// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;

namespace MySync.Server.Core.RequestHandlers
{
    /// <summary>
    /// VersionControl request handler.
    /// </summary>
    public static class VersionControl
    {
        public struct GetFilemapOutput
        {
            
        }

        /// <summary>
        /// GetFilemap processor-method.
        /// </summary>
        public static void GetFilemap(string body, HttpListenerResponse response)
        {
            var json = JObject.Parse(body);

            var token = json["token"];
            var projectName = json["project"];
            var commitId = json["commitId"];

            // TODO: validate token and check permisions

            //var outputData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(output));
            //response.OutputStream.Write(outputData, 0, outputData.Length);
        }

        public static void Push(HttpListenerRequest request, HttpListenerResponse response)
        {
            using (var reader = new BinaryReader(request.InputStream))
            {
                // TODO: header with json data
                using (var fs = File.OpenWrite("test.dat"))
                {
                    int read;
                    var buffer = new byte[64*1024];
                    while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fs.Write(buffer, 0, read);
                    }
                }
            }
        }

        public static void Pull(string body, HttpListenerResponse response)
        {

        }
    }
}
