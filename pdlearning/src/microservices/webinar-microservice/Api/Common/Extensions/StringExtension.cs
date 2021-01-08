using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.Webinar.Common.Extensions
{
    public static class StringExtension
    {
        public static string ConvertIpAddressToPattern(this string value)
        {
            return value?.Replace(".", string.Empty);
        }
    }
}
