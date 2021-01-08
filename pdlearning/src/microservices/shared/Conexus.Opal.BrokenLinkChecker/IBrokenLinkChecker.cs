using System.Collections.Generic;
using System.Threading.Tasks;

namespace Conexus.Opal.BrokenLinkChecker
{
    public interface IBrokenLinkChecker
    {
        /// <summary>
        /// Check an URL is online or not.
        /// </summary>
        /// <param name="url">Link.</param>
        /// <param name="whiteListDomain">The listed domain will be skipped to check. The URLs belong to this list always is valid.</param>
        /// <param name="maximumAutomaticRedirections">Maximum time to follow redirect see <see cref="System.Net.HttpWebRequest.MaximumAutomaticRedirections"/>.</param>
        /// <returns>Valid if the URL's response status code equals 200 or 206.</returns>
        Task<LinkCheckStatus> CheckUrlAsync(string url, List<string> whiteListDomain = null, int maximumAutomaticRedirections = 2);

        IReadOnlyList<string> ExtractUrlFromHtml(string htmlContent);
    }
}
