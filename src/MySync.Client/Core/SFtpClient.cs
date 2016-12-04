// MySync © 2016 Damian 'Erdroy' Korczowski
// under GPL-3.0 license

using System.IO;
using System.Text;
using Renci.SshNet;

namespace MySync.Client.Core
{
    public class SFtpClient
    {
        private SshClient _ssh;
        private SftpClient _sftp;

        public SFtpClient(string password, string address, ushort port = 22)
        {
            Open(password, address, port);
        }

        public void Open(string password, string address, ushort port = 22)
        {
            _ssh = new SshClient(address, port, "mysync", password);
            _ssh.Connect();

            _sftp = new SftpClient(address, port, "mysync", password);
            _sftp.Connect();

            // TODO: Error handling
        }

        public void Close()
        {
            _ssh.Disconnect();
            _ssh.Dispose();

            _sftp.Disconnect();
            _sftp.Dispose();
        }

        public string Execute(string cmd)
        {
            var command = _ssh.CreateCommand(cmd, Encoding.UTF8);
            command.Execute();

            return command.Error.Length > 2 ? command.Error : command.Result;
        }

        public void DeleteFile(string file)
        {
            
        }

        public void DeleteEmptyDirs(string path)
        {
            Execute("cd " + path);
            Execute("find . -type d -empty -delete");
        }

        public void DownloadFile(string outputFile, string remoteFile)
        {
            using (var fileStream = new FileStream(outputFile, FileMode.Create))
            {
                _sftp.DownloadFile(remoteFile, fileStream); // TODO: Progress
            }
        }

        public void UploadFile(string inputFile, string remoteFile)
        {
            // check dir
            var path = "";
            var i = remoteFile.Length-1;
            while (true)
            {
                if (i < 5)
                    return;

                if (remoteFile[i] == '/')
                    break;

                i--;
            }
            path = remoteFile.Substring(0, i);

            // TODO: Optimize directory structure building process
            if (!_sftp.Exists(path))
                _sftp.CreateDirectory(path);

            using (var fileStream = new FileStream(inputFile, FileMode.Open))
            {
                _sftp.UploadFile(fileStream, remoteFile, true); // TODO: Progress
            }
        }

        public void Upload(string text, string remoteFile)
        {
            var data = Encoding.UTF8.GetBytes(text);
            using (var stream = new MemoryStream(data))
            {
                _sftp.UploadFile(stream, remoteFile, true); // TODO: Progress
            }
        }
    }
}