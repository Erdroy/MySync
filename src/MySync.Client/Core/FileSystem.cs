// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

namespace MySync.Client.Core
{
    public class FileSystem
    {
        public SFtpClient Client { get; private set; }

        internal FileSystem() { }

        public void Open(SFtpClient client)
        {
            Client = client;
        }

        public void Close()
        {
            Client.Close();
        }

        public void BuildLocalFilemap()
        {
            
        }
        
        public string[] GetChangedFiles()
        {
            return null;
        }

        public void LockFile(string file)
        {

        }

        public void UnlockFile(string file)
        {

        }

        public void AddChanges(string file)
        {
            
        }

        public void RemoveChanges(string file)
        {

        }

        public void Discard(string file)
        {
            
        }

        public void Push(string message)
        {
            
        }

        public void Pull()
        {
            
        }

        public void LockProject()
        {
            
        }

        public void Update()
        {

        }

        public string[] Excluded { get; private set; }
    }
}