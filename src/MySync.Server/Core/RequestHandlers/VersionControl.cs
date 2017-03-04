// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MySync.Shared.RequestHeaders;
using MySync.Shared.VersionControl;

namespace MySync.Server.Core.RequestHandlers
{
    /// <summary>
    /// VersionControl request handler.
    /// </summary>
    public static class VersionControl
    {
        public static void Push(HttpListenerRequest request, HttpListenerResponse response)
        {
            using (var reader = new BinaryReader(request.InputStream))
            {
                using (var writer = new BinaryWriter(response.OutputStream))
                {
                    try
                    {
                        var authority = ProjectAuthority.FromJson(Encoding.UTF8.GetString(
                            reader.ReadBytes(reader.ReadInt32())
                        ));

                        // validate project name, password and check permisions from clientData
                        var projectSettings = ServerCore.Settings.Projects.FirstOrDefault(
                            x => x.Name == authority.ProjectName
                        );

                        // check if requested project exists
                        if (projectSettings == null)
                        {
                            writer.Write("Failed - project not found!");
                            return;
                        }

                        // check if user has the authority to this project
                        if (!projectSettings.AccessTokens.Contains(authority.AccessToken))
                        {
                            // do not tell that the project even exists
                            writer.Write("Failed - project not found!");
                            return;
                        }

                        // read commit
                        var commitData = reader.ReadBytes(reader.ReadInt32());
                        var commit = Commit.FromJson(Encoding.UTF8.GetString(commitData));

                        // read and save filemap
                        var filemapData = reader.ReadBytes(reader.ReadInt32());
                        File.WriteAllText("data/" + projectSettings.Name + "/filemap.json", Encoding.UTF8.GetString(filemapData));

                        // read data file
                        using (var fs = File.OpenWrite("temp.zip"))
                        {
                            int read;
                            var buffer = new byte[64*1024];
                            while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                fs.Write(buffer, 0, read);
                            }
                        }

                        // downloaded
                        // now apply changes
                        commit.Apply("data/" + projectSettings.Name, "temp.zip");

                        // add commit to database
                        
                        // return message
                        writer.Write("Done!");
                        Console.WriteLine("User '" + authority.Username + "' pushed changes!");
                    }
                    catch
                    {
                        writer.Write("Failed - invalid protocol/connection error!");
                    }
                }
            }
        }

        public static void Pull(string body, HttpListenerResponse response)
        {

        }
    }
}
