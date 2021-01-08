using System.Text;

namespace Microservice.Uploader.Signers
{
    /// <summary>
    /// Various Http helper routines.
    /// </summary>
    public static class AmazonHttpHelpers
    {
        /// <summary>
        /// Helper routine to url encode canonicalized header names and values for safe
        /// inclusion in the pre-signed url.
        /// </summary>
        /// <param name="data">The string to encode.</param>
        /// <param name="isPath">Whether the string is a URL path or not.</param>
        /// <returns>The encoded string.</returns>
#pragma warning disable CA1055 // URI-like return values should not be strings
        public static string UrlEncode(string data, bool isPath = false)
#pragma warning restore CA1055 // URI-like return values should not be strings
        {
            // The Set of accepted and valid Url characters per RFC3986. Characters outside of this set will be encoded.
            const string validUrlCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

            var encoded = new StringBuilder(data.Length * 2);
            string unreservedChars = string.Concat(validUrlCharacters, isPath ? "/:" : string.Empty);

            foreach (var b in Encoding.UTF8.GetBytes(data))
            {
                var symbol = (char)b;
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    encoded.Append(symbol);
                }
                else
                {
                    encoded.Append("%").Append($"{(int)symbol:X2}");
                }
            }

            return encoded.ToString();
        }
    }
}
