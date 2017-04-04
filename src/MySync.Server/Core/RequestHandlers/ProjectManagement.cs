// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.IO;
using System.Net;
using MySync.Shared.RequestHeaders;
using Newtonsoft.Json;

namespace MySync.Server.Core.RequestHandlers
{
    /// <summary>
    /// ProjectManagement request handler.
    /// </summary>
    public static class ProjectManagement
    {
        public static void Authorize(string body, HttpListenerResponse response)
        {
            var input = JsonConvert.DeserializeObject<ProjectAuthority>(body);

            using (var writer = new BinaryWriter(response.OutputStream))
            {
                if (Authorization.HasAuthority(input.AccessToken, input.ProjectName))
                {
                    writer.Write("{ \"auth\" : true }");
                    return;
                }

                writer.Write("{ \"auth\" : false }");
            }
        }
    }
}
