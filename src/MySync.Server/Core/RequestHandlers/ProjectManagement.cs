// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System.IO;
using System.Linq;
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
                // validate project name, password and check permisions from clientData
                var projectSettings = ServerCore.Settings.Projects.FirstOrDefault(
                    x => x.Name == input.ProjectName
                );

                // check if requested project exists
                if (projectSettings == null)
                {
                    writer.Write("{ \"auth\" : false }");
                    return;
                }

                // check if user has the authority to this project
                if (!projectSettings.AccessTokens.Contains(input.AccessToken))
                {
                    // do not tell that the project even exists
                    writer.Write("{ \"auth\" : false }");
                    return;
                }

                writer.Write("{ \"auth\" : true }");
            }
        }
    }
}
