using System;
using System.Net;

namespace Conexus.Opal.BrokenLinkChecker.Helper
{
    public static class LinkCheckHelper
    {
        public static string GetDescription(HttpStatusCode httpStatus)
        {
            var statusCode = (int)httpStatus;
            if ((statusCode >= 300 && statusCode < 400) || statusCode == 404)
            {
                return "The system cannot find the link. The web page could be moved or no longer exists.";
            }

            if (httpStatus == HttpStatusCode.RequestTimeout || httpStatus == HttpStatusCode.GatewayTimeout)
            {
                return "The server which host the link is taking too long to response.";
            }

            return "The system cannot reach the link.";
        }
    }
}
