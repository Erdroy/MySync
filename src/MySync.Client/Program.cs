// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using MySync.Client.Core;
using MySync.Shared.VersionControl;

namespace MySync.Client
{
    internal static class Program
    {
        private static void Main()
        {
            var project = Project.Open("D:\\SampleProject");
            var diff = project.BuildDiff();
            var commit = Commit.FromDiff(diff);

            project.BuildCommit(commit);
        }
    }
}
