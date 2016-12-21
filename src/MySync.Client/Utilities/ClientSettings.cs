// MySync © 2016 Damian 'Erdroy' Korczowski


using System.Collections.Generic;
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
        }

        public static ClientSettings Instance;

        public string Host { get; set; }

        public string Password { get; set; }

        public string MainDirectory { get; set; }

        public OpenedProject[] OpenedProjects;

        public ClientSettings()
        {
            MainDirectory = "/home/mysync";
        }

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