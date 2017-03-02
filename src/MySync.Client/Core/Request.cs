// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.IO;
using System.Net;
using System.Text;

namespace MySync.Client.Core
{
    /// <summary>
    /// Request class.
    /// Allows POST HTTP requests.
    /// </summary>
    public static class Request
    {
        /// <summary>
        /// Send HTTP POST request.
        /// </summary>
        /// <param name="address">The full URL addres.</param>
        /// <param name="body">The JSON body.</param>
        /// <param name="callback">The callback stream.</param>
        public static void Send(string address, string body, Action<Stream> callback)
        {
            var request = WebRequest.Create(address);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "POST";
            request.ContentType = "application/json";

            var data = Encoding.UTF8.GetBytes(body);
            request.ContentLength = data.Length;
            var dataStream = request.GetRequestStream();
            dataStream.Write(data, 0, data.Length);
            dataStream.Close();

            var response = request.GetResponse();
            callback(response.GetResponseStream());
        }
    }
}
