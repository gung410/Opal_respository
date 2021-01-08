using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LearnerApp.Common
{
    public static class ExtractUrlFromContent
    {
        public static List<string> Extract(string htmlContent)
        {
            List<string> list = new List<string>();
            if (htmlContent.IsNullOrEmpty())
            {
                return list;
            }

            Regex regex = new Regex("(?:href|src)=[\"|']?(.*?)[\"|'|>]+", RegexOptions.Singleline | RegexOptions.CultureInvariant);
            if (regex.IsMatch(htmlContent))
            {
                foreach (Match match in regex.Matches(htmlContent))
                {
                    if (match.Groups[1].Value.CheckURLValid())
                    {
                        list.Add(match.Groups[1].Value);
                    }
                }
            }

            return list;
        }

        public static List<string> ExtractImageUrl(string htmlContent)
        {
            List<string> list = new List<string>();

            MatchCollection matches = Regex.Matches(htmlContent, @"<img[^>]*?src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*?>");
            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    if (m.Groups[1].Value.CheckURLValid())
                    {
                        list.Add(m.Groups[1].Value);
                    }
                }
            }

            return list;
        }

        private static bool CheckURLValid(this string source) => Uri.TryCreate(source, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttps || uriResult.Scheme == Uri.UriSchemeHttp);
    }
}
