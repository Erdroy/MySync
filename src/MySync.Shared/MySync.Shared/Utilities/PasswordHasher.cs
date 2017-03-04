// MySync © 2016-2017 Damian 'Erdroy' Korczowski

using System;
using System.Security.Cryptography;
using System.Text;

namespace MySync.Shared.Utilities
{
    /// <summary>
    /// PasswordHasher class.
    /// </summary>
    public class PasswordHasher
    {
        public static string Hash(string username, string password)
        {
            // Calculate MD5 hash. This requires that the string is splitted into a byte[].
            MD5 md5 = new MD5CryptoServiceProvider();
            var textToHash = Encoding.Default.GetBytes(password + username);
            var result = md5.ComputeHash(textToHash);

            // Convert result back to string.
            return BitConverter.ToString(result);
        }
    }
}
