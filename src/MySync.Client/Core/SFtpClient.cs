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

        public void DownloadFile(string outputFile, string remoteFile)
        {
            using (var fileStream = new FileStream(outputFile, FileMode.Create))
            {
                _sftp.DownloadFile(remoteFile, fileStream);
            }
        }
    }
}