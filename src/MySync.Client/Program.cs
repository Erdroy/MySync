// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Net;
using System.Threading;
using MySync.Client.Core;

namespace MySync.Client
{
    internal static class Program
    {
        private static void Main()
        {
            // initialize
            var body = "{\n\t\"username\":\"testUser\",\n\t\"password\":\"235t2b62626256236n2===\"\n}";
            Request.Send("http://127.0.0.1:8080/authorize", body, stream =>
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    Console.WriteLine(reader.ReadToEnd());
                }
            });

            while (true)
                Thread.Sleep(100);
        }
    }
}
