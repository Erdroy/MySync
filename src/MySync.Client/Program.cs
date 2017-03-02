// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Threading;
using MySync.Client.Core;
using MySync.Shared.VersionControl;
using Newtonsoft.Json.Linq;

namespace MySync.Client
{
    internal static class Program
    {
        private static void Main()
        {
            // initialize
            Request.Send("http://127.0.0.1:8080/check", "", stream =>
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    var json = JObject.Parse(reader.ReadToEnd());

                    if (json["status"].ToObject<int>() == 0)
                    {
                        Console.WriteLine(@"MySync server is available!");
                    }
                }
            });

            Console.WriteLine(@"Loading 'Sample' project...");
            var filemap = Filemap.Build("D:\\TestProject");

            Console.WriteLine(@"Calculating changes...");

            while (true)
                Thread.Sleep(100);
        }
    }
}
