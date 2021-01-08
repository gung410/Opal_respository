using System;
using System.Text.RegularExpressions;

namespace cxOrganization.Domain.Common
{
    public static class RegexUtils
    {
        private const string EmailRegexPattern = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";

        private const string quote ="\"";
        private static string DomainEmailPattern(string domain)
        {
            return @"^[a-zA-Z0-9_.+-]+@(?:(?:[a-zA-Z0-9-]+\.)?[a-zA-Z]+\.)?(" + domain + ")\\.com$";
        }

        public static bool IsValidEmailFormat(string email)
        {
            return Regex.IsMatch(email,
                                EmailRegexPattern,
                                RegexOptions.IgnoreCase,
                                TimeSpan.FromMilliseconds(250));
        }

        public static bool IsValidDomainEmail(string email, string domain)
        {
            return Regex.IsMatch(email,
                                DomainEmailPattern(domain),
                                RegexOptions.IgnoreCase,
                                TimeSpan.FromMilliseconds(250));
        }

        public static bool isMatchPatternInRangeOfWords(string words, string pattern)
        {
            return Regex.Matches(words, pattern).Count > 0;
        }
    }
}
