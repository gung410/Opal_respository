using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using cxPlatform.Core.Exceptions;
using cxPlatform.Core.Extentions.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace cxOrganization.WebServiceAPI.Extensions
{
    public static class HttpExtension
    {
        
        public static (int, int) GetCustomerAndOnwerIds(this HttpRequest request)
        {
            StringValues headerValues;
            string token = "";
            if (request.Headers.TryGetValue("cxtoken", out headerValues))
                token = headerValues.FirstOrDefault();

            if (!string.IsNullOrEmpty(token))
            {
                string[] elements = token.Split(':');
                if(elements.Length == 2)
                {
                    if (int.TryParse(elements[0], out var ownerId) && int.TryParse(elements[1], out var customerId))
                        return (ownerId, customerId);
                }
            }
            throw new CXValidationException(cxExceptionCodes.ERROR_CXTOKEN_INVALID);
        }

        public static string GetUserIdFromCXID(this HttpContext context)
        {
            try
            {
                var sub = context.GetSub();
                if (!string.IsNullOrEmpty(sub)) return sub;

                var accessToken = context.Request.GetAuthorization().Replace("Bearer ", "");
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                var userId = tokenHandler.ReadJwtToken(accessToken).Claims.First(claim => claim.Type == "sub").Value;

                return userId;
            }
            catch
            {
                //return as default
                return null;
            }                 
        }

        public static string GetClientRequestIP(this HttpContext httpContext, bool tryUseXForwardHeader = true)
        {
            string ip = null;

            // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

            // X-Forwarded-For (csv list):  Using the First entry in the list seems to work
            // for 99% of cases however it has been suggested that a better (although tedious)
            // approach might be to read each IP from right to left and use the first public IP.
            // http://stackoverflow.com/a/43554000/538763
            //
            if (tryUseXForwardHeader)
                ip = SplitCsv(httpContext.GetHeaderValueAs<string>("X-Forwarded-For")).FirstOrDefault();

            // RemoteIpAddress is always null in DNX RC1 Update1 (bug).
            if (string.IsNullOrWhiteSpace(ip) && httpContext?.Connection?.RemoteIpAddress != null)
                ip = httpContext.Connection.RemoteIpAddress.ToString();

            if (string.IsNullOrWhiteSpace(ip))
                ip = httpContext.GetHeaderValueAs<string>("REMOTE_ADDR");
            return ip;
        }

        public static T GetHeaderValueAs<T>(this HttpContext httpContext, string headerName)
        {
            StringValues values;

            if (httpContext?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
            {
                string rawValues = values.ToString(); // writes out as Csv when there are multiple.

                if (!string.IsNullOrWhiteSpace(rawValues))
                    return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }

            return default(T);
        }

        private static List<string> SplitCsv(string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }

        public static string GetClientId(this HttpContext httpContext)
        {
            return httpContext?.User?.FindFirst(c => string.Equals(c.Type,"client_id",StringComparison.CurrentCultureIgnoreCase))?.Value;
        }

        public static string GetSub(this HttpContext httpContext)
        {
            return httpContext?.User
                ?.FindFirst(c => string.Equals(c.Type, "sub", StringComparison.CurrentCultureIgnoreCase))?.Value;
        }

        public static bool ShouldLimitPageSize(this HttpRequest request)
        {
            return !"true".Equals(request.GetHeaderValue("x-no-limit-page-size"), StringComparison.CurrentCultureIgnoreCase);
        }
    }
}