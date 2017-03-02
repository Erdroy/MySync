// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.IO;
using System.Threading;
using MySync.Client.Core;

namespace MySync.Client
{
    internal static class Program
    {
        private static void Main()
        {
            // initialize
            /*Request.Send("http://127.0.0.1:8080/check", "", stream =>
            {
                using (var reader = new System.IO.StreamReader(stream))
                {
                    var json = JObject.Parse(reader.ReadToEnd());

                    if (json["status"].ToObject<int>() == 0)
                    {
                        Console.WriteLine(@"MySync server is available!");
                    }
                }
            });*/

            using (var file = new FileStream("testdata.dat", FileMode.Open))
            {
                var dataStream = Request.BeginSend("http://127.0.0.1:8080/push", file.Length);

                int read;
                var buffer = new byte[64 * 1024];
                while ((read = file.Read(buffer, 0, buffer.Length)) > 0)
                {
                    dataStream.Write(buffer, 0, read);
                }

                Request.EndSend(stream =>
                {
                    Console.WriteLine(@"Uploaded!");
                });
            }

            while (true)
                Thread.Sleep(100);
        }
    }
}
