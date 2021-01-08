using System;
using System.Security.Cryptography;
using System.Text;

namespace Thunder.Platform.Core.Helpers
{
    public static class EncryptionHelper
    {
        public static string ComputeSha1Hash(string plainText)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(plainText);
            byte[] hashBytes = new SHA1CryptoServiceProvider().ComputeHash(bytes);

            // Convert the encrypted bytes back to a string (base 16)
            StringBuilder hashString = new StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashString.Append(Convert.ToString(hashBytes[i], 16).PadLeft(2, '0'));
            }

            return hashString.ToString().PadLeft(32, '0');
        }
    }
}
