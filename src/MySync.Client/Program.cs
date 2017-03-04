// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using MySync.Client.Core;
using MySync.Shared.RequestHeaders;
using MySync.Shared.Utilities;
using MySync.Shared.VersionControl;

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

            var project = Project.Open("D:\\SampleProject");

            project.Authority = new ProjectAuthority
            {
                ProjectName = "SampleProject",
                Username = username,
                AccessToken = token
            };

            var diff = project.BuildDiff();
            var commit = Commit.FromDiff(diff);
            var datafile = project.BuildCommit(commit);

            project.PushCommit(commit, datafile);
        }
    }
}
