// MySync © 2016 Damian 'Erdroy' Korczowski

using System.IO;
using Newtonsoft.Json;

namespace MySync.Client.Utilities
{

    public class ClientSettings
    {
        public class OpenedProject
        {
            public string Name;
            public string LocalDir;
            public string Host;
            public string Password;
        }

        public static ClientSettings Instance;
        
        public OpenedProject[] OpenedProjects;
        
        public static void Load()
        {
            if (!File.Exists("config.json"))
            {
                var settings = new ClientSettings();
                settings.Save();
                Instance = settings;
            }

            var json = File.ReadAllText("config.json");
            Instance = JsonConvert.DeserializeObject<ClientSettings>(json, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            });

            File.WriteAllText("config.json", json);
        }
    }
}