// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Collections.Generic;
using System.IO;
using MySync.Client.Core;
using MySync.Shared.RequestHeaders;
using MySync.Shared.Utilities;
using MySync.Shared.VersionControl;
using Newtonsoft.Json;

namespace MySync.Client
{
    internal static class Program
    {
        private static void Main()
        {
            Console.Write(@"Username: ");
            var username = Console.ReadLine();

            Console.Write(@"Access token: ");
            var token = PasswordHasher.Hash(username, Console.ReadLine());

            var settings = JsonConvert.DeserializeObject<ClientSettings>(File.ReadAllText("client.json"));
            
            Console.WriteLine(@"Select project: ");
            for (var i = 0; i < settings.Projects.Length; i++)
            {
                Console.WriteLine(i + @") " + settings.Projects[i].Name);
            }

            Console.Write(@"Project: ");
            var projectSettings = settings.Projects[int.Parse(Console.ReadLine())];

            Console.WriteLine(@"Using project: " + projectSettings.Name);
            var project = Project.OpenWorkingCopy(projectSettings.Address, projectSettings.RootDir);

            project.Authority = new ProjectAuthority
            {
                ProjectName = projectSettings.Name,
                Username = username,
                AccessToken = token
            };

            Console.WriteLine(@"Select: PUSH or PULL");
            var op = Console.ReadLine();

            if (op == "PUSH")
            {
                var diffs = project.BuildDiff();
                var diff = new List<Filemap.FileDiff>();

                Console.WriteLine(@"Select files: ");
                foreach (var file in diffs)
                {
                    Console.WriteLine(file.FileName + @" - operation: " + file.DiffType + @" - [ENTER to ACCEPT] or [ANY key and ENTER to IGNORE]");
                    if (Console.ReadLine().Length == 0)
                    {
                        diff.Add(file);
                    }
                }

                if (diff.Count == 0)
                {
                    Console.WriteLine(@"Press any key to exit...");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine(@"Press any key to PUSH...");
                Console.ReadLine();

                var commit = Commit.FromDiff(diff.ToArray());
                var datafile = commit.Build(project.RootDir, project.RootDir + ".mysync\\commit.zip");

                project.Push(commit, datafile);
            }
            else if(op == "PULL")
            {
                project.Pull();
            }
            
            Console.WriteLine(@"Press any key to exit...");
            Console.ReadLine();
        }
    }
}

