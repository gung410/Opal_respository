using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.Course.Common.Extensions
{
    public static class StringExtension
    {
        public static string TakeFirst(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.Substring(0, Math.Min(str.Length, maxLength));
        }

        public static string TrimAllSpaces(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            return str.Replace(" ", string.Empty);
        }
    }
}
