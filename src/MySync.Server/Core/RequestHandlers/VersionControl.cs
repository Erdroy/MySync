// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MySync.Server.Core.DatabaseModels;
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
                        using (var fs = File.Create("temp.zip"))
                        {
                            int read;
                            var buffer = new byte[64*1024];
                            while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                fs.Write(buffer, 0, read);
                            }
                        }

                        // --- from now - this part CAN'T fail, if so, the whole project may be incorrect after this!

                        // TODO: make commited files backups and restore when failed to unpack the commit

                        var commitId = 0;
                        try
                        {
                            // downloaded
                            // now apply changes
                            commit.Apply("data/" + projectSettings.Name, "temp.zip");

                            // add commit to projects database
                            var commits = ServerCore.Database.GetCollection<CommitModel>(projectSettings.Name);
                            commitId = commits.Count()+1;
                            
                            // build commit
                            var commitModel = new CommitModel
                            {
                                CommitId = commitId,
                                CommitDescription = commit.Description,
                                Files = new CommitModel.FileDiff[commit.Files.Length]
                            };

                            for (var i = 0; i < commit.Files.Length; i++)
                            {
                                var file = commit.Files[i];

                                commitModel.Files[i] = new CommitModel.FileDiff
                                {
                                    Name = file.FileName,
                                    Version = file.Version,
                                    Operation = (int) file.DiffType
                                };
                            }

                            // insert
                            commits.Insert(commitModel);
                        }
                        catch
                        {
                            writer.Write("#RESTORE Failed - error when updating project!");
                        }
                        
                        // ok, we are out of the danger zone.

                        // return message
                        writer.Write("Done!");
                        writer.Write(commitId);
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
            using (var writer = new BinaryWriter(response.OutputStream))
            {
                try
                {
                    var authority = ProjectAuthority.FromJson(body);

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

                    // read all commits for latest downloaded client-commit
                    // diff all commits
                    // send commit diff
                    // send commit diff data file
                }
                catch
                {
                    writer.Write("Failed - invalid protocol/connection error!");
                }
            }
        }
    }
}
