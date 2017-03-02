// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using MySync.Server.Core;

namespace MySync.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // run server
            using (var core = new ServerCore())
            {
                core.Run();
            }
        }
    }
}
