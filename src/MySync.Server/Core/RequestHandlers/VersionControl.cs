﻿// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using MongoDB.Driver;
using MySync.Server.Core.DatabaseModels;
using MySync.Shared.Core;
using MySync.Shared.RequestHeaders;
using MySync.Shared.VersionControl;
using Newtonsoft.Json;
// ReSharper disable AccessToDisposedClosure

namespace MySync.Server.Core.RequestHandlers
{
    /// <summary>
    /// VersionControl request handler.
    /// </summary>
    public static class VersionControl
    {
        public static void Push(HttpListenerRequest request, HttpListenerResponse response)
        {
            var projectName = "";

            try
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
                            
                            projectName = authority.ProjectName;

                            // validate project name, password and check permisions from clientData
                            if (!Authorization.HasAuthority(authority.AccessToken, projectName))
                            {
                                writer.Write("Failed - project not found!");
                                return;
                            }

                            // request project lock
                            if (ProjectLock.TryLock(projectName, ProjectLock.LockMode.Upload) !=
                                ProjectLock.LockMode.None)
                            {
                                writer.Write("Failed - project is locked!");
                                return;
                            }

                            // read commit
                            var commitData = reader.ReadBytes(reader.ReadInt32());
                            var commit = Commit.FromJson(Encoding.UTF8.GetString(commitData));

                            var hasFile = reader.ReadBoolean();

                            if (hasFile)
                            {
                                Console.WriteLine("Receiving file...");
                                try
                                {
                                    // read data file
                                    using (var fs = File.Create("temp_recv.zip"))
                                    {
                                        try
                                        {
                                            int read;
                                            var buffer = new byte[64 * 1024];
                                            while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                                            {
                                                fs.Write(buffer, 0, read);
                                            }
                                        }
                                        catch
                                        {
                                            // user lost connection or closed the client 
                                            // before the whole data file arrived
                                            Console.WriteLine("User '" + authority.Username + "' canceled commit upload.");
                                            ProjectLock.Unlock(projectName);
                                            return;
                                        }
                                    }
                                }
                                catch
                                {
                                    // user lost connection or closed the client 
                                    // before the whole data file arrived
                                    Console.WriteLine("User '" + authority.Username + "' commit upload failed.");
                                    ProjectLock.Unlock(projectName);
                                    return;
                                }
                            }

                            // --- from now - this part CAN'T fail, if so, the whole project may be incorrect after this!

                            var projectDir = "data/" + projectName;

                            // make commited files backup
                            commit.Backup(projectDir);

                            int commitId;
                            try
                            {
                                // downloaded
                                // now apply changes
                                commit.Apply(projectDir, "temp_recv.zip", hasFile);

                                // add commit to projects database
                                var projectCollection =
                                    ServerCore.Database.GetCollection<CommitModel>(projectName);
                                commitId = (int) projectCollection.Count(FilterDefinition<CommitModel>.Empty) + 1;

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
                                projectCollection.InsertOne(commitModel);

                                // delete zip file if exists
                                if (hasFile)
                                    File.Delete("temp_recv.zip");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Failed to apply commit from user '" + authority.Username + "'");
                                writer.Write("#RESTORE Failed - error when updating project! Error: " + ex);

                                // restore backup
                                commit.RestoreBackup(projectDir);

                                // UNLOCK
                                ProjectLock.Unlock(projectName);
                                return;
                            }

                            // ok, we are out of the danger zone.

                            // return message
                            writer.Write("Done!");
                            writer.Write(commitId);

                            // clean backups
                            commit.CleanBackups(projectDir);

                            Console.WriteLine("User '" + authority.Username + "' pushed changes!");
                        }
                        catch (Exception ex)
                        {
                            writer.Write("Failed - invalid protocol/connection error! Error: " + ex);
                            Console.WriteLine("PUSH failed");
                            ProjectLock.Unlock(projectName);
                        }
                    }
                }
            }
            catch
            {
                // this shouldn't be possible, 
                // but anyway handle exceptions here
                Console.WriteLine("PUSH failed");
                ProjectLock.Unlock(projectName);
            }

            Console.WriteLine("Push done");
            ProjectLock.Unlock(projectName);
        }

        public static void Pull(string body, HttpListenerResponse response)
        {
            var projectName = "";
            using (var writer = new BinaryWriter(response.OutputStream))
            {
                try
                {
                    var input = JsonConvert.DeserializeObject<PullInput>(body);
                    projectName = input.Authority.ProjectName;

                    if (!Authorization.HasAuthority(input.Authority.AccessToken, projectName))
                    {
                        writer.Write(false);
                        writer.Write("Failed - project not found!");
                        return;
                    }

                    // request project lock
                    if (ProjectLock.TryLock(projectName, ProjectLock.LockMode.Upload) == ProjectLock.LockMode.Any)
                    {
                        writer.Write(false);
                        writer.Write("Failed - project is locked!");
                        return;
                    }

                    // find latest downloaded client-commit
                    var commitId = input.CommitId + 1; // the first commit id which will be downloaded if exists
                    var projectCollection = ServerCore.Database.GetCollection<CommitModel>(projectName);

                    // find all needed commits that will be used to calculate diff
                    var commits = projectCollection.Find(x => x.CommitId >= commitId).ToList();

                    // check if there is at least one commit, if not, break the pull request
                    if (!commits.Any())
                    {
                        writer.Write(false);
                        writer.Write("No files to download.");
                        // UNLOCK
                        ProjectLock.Unlock(projectName);
                        return;
                    }

                    writer.Write(true);

                    // diff all commits
                    var commit = commits[0].ToCommit();

                    foreach (var t in commits)
                    {
                        commit.Add(t.ToCommit());
                    }

                    var fileNeeded = commit.IsUploadNeeded();

                    // build commit diff data file
                    var dir = "data/" + projectName + "/";

                    var tempDataFile = "temp_send_"+ input .Authority.Username+ ".zip";

                    if (fileNeeded) // build commit zip if needed
                    {
                        var lastSend = DateTime.Now;
                        commit.Build(dir, tempDataFile, delegate (int progress)
                        {
                            if ((DateTime.Now - lastSend).TotalSeconds >= 1.0f)
                            {
                                writer.Write(false);
                                writer.Write(progress);
                            }
                        });
                    }

                    writer.Write(true);

                    // send commit diff
                    var commitJson = commit.ToJson();
                    writer.Write(commitJson);

                    // send commit id
                    writer.Write(commits[commits.Count - 1].CommitId);

                    // write file upload indicator
                    writer.Write(fileNeeded);

                    if (fileNeeded)
                    {

                        // send commit diff data file
                        using (var file = new FileStream(tempDataFile, FileMode.Open))
                        {
                            // write file size
                            writer.Write(file.Length);

                            int read;
                            var buffer = new byte[64*1024];
                            try
                            {
                                while ((read = file.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    response.OutputStream.Write(buffer, 0, read);
                                }
                            }
                            catch
                            {
                                // user lost connection or closed the client 
                                // before the whole data file is sent
                                Console.WriteLine("User '" + input.Authority.Username + "' canceled commit download.");
                                ProjectLock.Unlock(projectName);
                                return;
                            }
                        }
                        File.Delete(tempDataFile);
                    }
                }
                catch (Exception ex)
                {
                    writer.Write("Failed - invalid protocol/connection error! Error: " + ex);
                    ProjectLock.Unlock(projectName);
                }
            }
            ProjectLock.Unlock(projectName);
        }
        
        public static void Discard(string body, HttpListenerResponse response)
        {
            var projectName = "";

            using (var writer = new BinaryWriter(response.OutputStream))
            {
                try
                {
                    var input = DiscardInput.FromJson(body);
                    
                    projectName = input.Authority.ProjectName;
                    if (!Authorization.HasAuthority(input.Authority.AccessToken, projectName))
                    {
                        writer.Write("Failed - project not found!");
                        return;
                    }

                    // request project lock
                    if (ProjectLock.TryLock(projectName, ProjectLock.LockMode.Upload) == ProjectLock.LockMode.Any)
                    {
                        writer.Write("Failed - project is locked!");
                        return;
                    }

                    var projectCollection = ServerCore.Database.GetCollection<CommitModel>(projectName);
                    var commits = projectCollection.Find(x => x.CommitId >= 0).ToList().OrderBy(x => x.CommitId).ToArray();

                    // select files then pack them
                    // and send to the client
                    var diff = new List<Filemap.FileDiff>();

                    foreach (var file in input.Files)
                    {
                        // select commits which contains this file(which is not removed by the commit) then order by commit and select the first commit.
                        var validCommit = commits.Where(x => x.Files.Any(n => n.Name == file.FileName && n.Operation != 2)).OrderByDescending(x => x.CommitId).FirstOrDefault();

                        if (validCommit != null)
                        {
                            // select the file
                            var validFile = validCommit.Files.First(x => x.Name == file.FileName);

                            diff.Add(new Filemap.FileDiff
                            {
                                FileName = validFile.Name,
                                DiffType = Filemap.FileDiff.Type.Changed,
                                Version = validFile.Version
                            });
                        }
                        else
                        {
                            Console.WriteLine("Commit not found for file " + file.FileName);
                        }
                    }

                    // create commit
                    var commit = Commit.FromDiff(diff.ToArray());

                    // write commit in json format
                    var commitJson = commit.ToJson();
                    writer.Write(commitJson);

                    // build commit diff data file
                    var dir = "data/" + projectName + "/";
                    var tempDataFile = "temp_send_" + input.Authority.Username + ".zip";
                    commit.Build(dir, tempDataFile, delegate (int progress)
                    {
                        var lastSend = DateTime.Now;
                        if ((DateTime.Now - lastSend).TotalSeconds >= 1.0f)
                        {
                            writer.Write(false);
                            writer.Write(progress);
                        }
                    });

                    writer.Write(true);

                    // send commit diff data file
                    using (var nfs = new NetFileStream(tempDataFile, onError: delegate
                    {
                        // user lost connection or closed the client 
                        // before the whole data file is sent
                        Console.WriteLine("User '" + input.Authority.Username + "' canceled commit download.");
                        ProjectLock.Unlock(projectName);
                    }))
                    {
                        nfs.Upload(response.OutputStream);
                    }

                    File.Delete(tempDataFile);
                }
                catch (Exception ex)
                {
                    writer.Write("Failed - invalid protocol/connection error! Error: " + ex);
                    Console.WriteLine(ex);
                    ProjectLock.Unlock(projectName);
                }
            }
            ProjectLock.Unlock(projectName);
        }

        public static void GetCommit(string body, HttpListenerResponse response)
        {
            using (var writer = new BinaryWriter(response.OutputStream))
            {
                try
                {
                    var input = JsonConvert.DeserializeObject<PullInput>(body);

                    if (!Authorization.HasAuthority(input.Authority.AccessToken, input.Authority.ProjectName))
                    {
                        writer.Write("Failed - project not found!");
                        return;
                    }

                    var projectCollection = ServerCore.Database.GetCollection<CommitModel>(input.Authority.ProjectName);
                    var itemCount = projectCollection.Count(FilterDefinition<CommitModel>.Empty);

                    if (itemCount > 0)
                    {
                        var lastCommit =
                            projectCollection.Find(x => true)
                                .SortByDescending(d => d.CommitId)
                                .Limit(1)
                                .FirstOrDefault();

                        writer.Write("Done");
                        writer.Write(lastCommit.CommitId);
                    }
                    else
                    {
                        writer.Write("Done");
                        writer.Write(-1);
                    }
                }
                catch (Exception ex)
                {
                    writer.Write("Failed - invalid protocol/connection error! Error: " + ex);
                }
            }
        }

    }
}