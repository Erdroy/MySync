// MySync © 2016 Damian 'Erdroy' Korczowski


using System.IO;
using System.Linq;
using System.Text;
using MySync.Client.Utilities;
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
            Execute("rm " + file);
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

        public string DownloadFile(string remoteFile)
        {
            using (var stream = new MemoryStream())
            {
                _sftp.DownloadFile(remoteFile, stream);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        public void UploadFile(string inputFile, string remoteFile)
        {
            // check dir
            var path = PathUtils.GetPath(remoteFile);

            // TODO: Optimize directory structure building process
            if (!_sftp.Exists(path))
                _sftp.CreateDirectory(path);
            
            using (var fileStream = new FileStream(inputFile, FileMode.Open))
            {
                _sftp.UploadFile(fileStream, remoteFile, true); // TODO: Progress
            }
        }

        public string[] ListFiles(string rootdir)
        {
            var objects = _sftp.ListDirectory(rootdir);
            return (from obj in objects where obj.IsRegularFile select obj.Name).ToArray();
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