using System;
using System.Globalization;

namespace cxOrganization.Domain.Extensions
{
    public static class StringExtension
    {
        public static bool EqualsIgnoreCase(this string a, string b)
        {
            return String.Equals(a, b, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string ToTitleCase(this string title)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title.ToLower());
        }

        public static DateTime? toDateTimeWithFormat(this string dateString, string format)
        {
            DateTime convertedDateTime;
            var result = DateTime.TryParseExact(dateString, 
                                                format,
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.None,
                                                out convertedDateTime);
            
            return result ? convertedDateTime : (DateTime?)null;
        }

        /// <summary>
        /// Ref: https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#indices-and-ranges
        /// </summary>
        /// <param name="originString"></param>
        /// <returns></returns>
        public static string RemoveTheLastChar(this string originString)
        {
            return originString[0..^1];
        }
    }
}
