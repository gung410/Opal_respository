using System;
using System.Collections.Generic;
using System.Linq;

namespace Microservice.Course.Infrastructure.Helpers
{
    public static class RandomHelper
    {
        private static readonly Random Random = new Random();

        public static string GenerateUniqueString(int length, List<string> existedStrings, string prefix = null)
        {
            var result = string.Empty;

            while (true)
            {
                const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

                var randomString = new string(Enumerable.Repeat(chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());

                result = string.IsNullOrEmpty(prefix) ? randomString : $"{prefix}-{randomString}";
                if (!existedStrings.Any() || !existedStrings.Contains(result))
                {
                    break;
                }
            }

            return result;
        }
    }
}
