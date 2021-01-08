using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Conexus.Opal.BrokenLinkChecker.Helper;
using Thunder.Platform.Core;

namespace Conexus.Opal.BrokenLinkChecker
{
    public class BrokenLinkChecker : IBrokenLinkChecker
    {
        public const string LinkPattern = @"<a(?:\s+|\s.+\s)href=""(?<url>.+?)""";

        public const string ImgSrcPattern = @"<img(?:\s+|\s.+\s)src=""(?<url>.+?)""";

        /// <summary>
        /// Normal url, for example: https://www.abc.com.
        /// </summary>
        public const string HostPattern = @"^(https?:)?(\/\/)?(www\.)?[-a-zA-Z0-9._-]{1,256}\.[a-zA-Z0-9]{1,6}";

        /// <summary>
        /// Internal page url, for example: #content-id.
        /// </summary>
        public const string AnchorLinkPattern = @"^#[-a-zA-Z]{1,}";

        /// <inheritdoc/>
        public async Task<LinkCheckStatus> CheckUrlAsync(string url, List<string> whiteListDomain, int maximumAutomaticRedirections = 3)
        {
            whiteListDomain ??= new List<string>();

            // In case of matching internal page url, return immediately.
            if (Regex.IsMatch(url, AnchorLinkPattern))
            {
                return LinkCheckStatus.ValidUrl();
            }

            // In case of not matching HostPattern, return invalid url.
            if (!Regex.IsMatch(url, HostPattern))
            {
                return LinkCheckStatus.InvalidUrl();
            }

            var hostMatches = Regex.Matches(url, HostPattern, RegexOptions.IgnoreCase);
            string host = hostMatches[0].Value;

            var isInWhitelist = whiteListDomain
                .AsEnumerable()
                .Any(p => p.Contains(host));

            if (isInWhitelist)
            {
                return LinkCheckStatus.ValidUrl();
            }

            // In case the URL does not contain a protocol, force protocol to https because our site always serves at https.
            if (url.ToLower().StartsWith("//"))
            {
                url = "https:" + url;
            }

            if (!url.ToLower().StartsWith("http://") && !url.ToLower().StartsWith("https://"))
            {
                url = "https://" + url;
            }

            var status = new LinkCheckStatus();
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = 30000; // 30 seconds
                req.MaximumAutomaticRedirections = maximumAutomaticRedirections;
                req.AllowAutoRedirect = true;
                var response = await req.GetResponseAsync();
                var httpResponse = (HttpWebResponse)response;

                switch (httpResponse.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.PartialContent:
                        status.IsValid = true;
                        break;
                    default:
                        status.InvalidReason = LinkCheckHelper.GetDescription(httpResponse.StatusCode);
                        break;
                }
            }
            catch (WebException exception)
            {
                var response = (HttpWebResponse)exception.Response;
                var statusCode = response?.StatusCode ?? HttpStatusCode.ServiceUnavailable;

                // Continue to check in case got exception when url redirect from HTTPS to HTTP.
                if ((statusCode == HttpStatusCode.MovedPermanently || statusCode == HttpStatusCode.Moved) && maximumAutomaticRedirections > 0)
                {
                    string redirectedUrl = response?.Headers[CommonHttpHeaderNames.Location];
                    if (!string.IsNullOrEmpty(redirectedUrl))
                    {
                        return await CheckUrlAsync(redirectedUrl, whiteListDomain, maximumAutomaticRedirections - 1);
                    }
                }

                status.InvalidReason = LinkCheckHelper.GetDescription(statusCode);
            }

            return status;
        }

        public IReadOnlyList<string> ExtractUrlFromHtml(string htmlContent)
        {
            if (string.IsNullOrEmpty(htmlContent))
            {
                return new List<string>();
            }

            var options = RegexOptions.Multiline | RegexOptions.IgnoreCase;
            var hrefMatches = Regex.Matches(htmlContent, LinkPattern, options);
            var srcMatches = Regex.Matches(htmlContent, ImgSrcPattern, options);
            var matches = hrefMatches.Concat(srcMatches);

            var urls = matches
                .Where(m => !string.IsNullOrEmpty(m.Groups["url"].Value))
                .Select(m => m.Groups["url"].Value)
                .Distinct()
                .ToList();

            return urls;
        }
    }
}
