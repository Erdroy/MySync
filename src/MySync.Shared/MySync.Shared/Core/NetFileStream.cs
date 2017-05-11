// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.IO;

namespace MySync.Shared.Core
{
    public delegate void OnStreamProgress(long totalBytes, long sentBytes);

    public sealed class NetFileStream : IDisposable
    {
        private readonly byte[] _buffer;
        private readonly string _filename;
        private readonly Action<Exception> _onError;

        private FileStream _fileStream;

        public NetFileStream(string filename, int bufferSize = 65535, Action<Exception> onError = null)
        {
            _buffer = new byte[bufferSize];
            _filename = filename;
            _onError = onError;
        }

        public void Upload(Stream output, OnStreamProgress onProgress = null)
        {
            try
            {
                _fileStream = new FileStream(_filename, FileMode.Open);

                var filelength = _fileStream.Length;

                using (var bs = new BinaryWriter(output))
                {
                    bs.Write(filelength);

                    int read;
                    long totalRead = 0u;
                    while ((read = _fileStream.Read(_buffer, 0, _buffer.Length)) > 0)
                    {
                        bs.Write(_buffer, 0, read);

                        totalRead += read;
                        onProgress?.Invoke(filelength, totalRead);
                    }
                }
            }
            catch (Exception ex)
            {
                _onError?.Invoke(ex);
            }
        }

        public void Download(Stream input, OnStreamProgress onProgress = null)
        {
            try
            {
                _fileStream = new FileStream(_filename, FileMode.Create);

                using (var bs = new BinaryReader(input))
                {
                    var filelength = bs.ReadInt64();

                    int read;
                    long totalRead = 0u;
                    while ((read = bs.Read(_buffer, 0, _buffer.Length)) > 0)
                    {
                        _fileStream.Write(_buffer, 0, read);

                        totalRead += read;
                        onProgress?.Invoke(filelength, totalRead);
                    }
                }
            }
            catch (Exception ex)
            {
                _onError?.Invoke(ex);
            }
        }

        public void Dispose()
        {
            _fileStream.Dispose();
        }
    }
}
