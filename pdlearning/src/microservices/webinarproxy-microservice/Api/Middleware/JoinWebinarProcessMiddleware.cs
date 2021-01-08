using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microservice.WebinarProxy.Application.Constants;
using Microservice.WebinarProxy.Application.Models;
using Microservice.WebinarProxy.Application.Services;
using Microservice.WebinarProxy.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microservice.WebinarProxy.Middleware
{
    public class JoinWebinarProcessMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMeetingUrlHelper _urlHelper;
        private readonly IChecksumHelper _checksumHelper;
        private readonly ProxyOptions _proxyOptions;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;
        private readonly BigBlueButtonOptions _bbbOptions;

        public JoinWebinarProcessMiddleware(
            RequestDelegate next,
            IMeetingUrlHelper urlHelper,
            IChecksumHelper checksumHelper,
            IOptions<BigBlueButtonOptions> bbbOptions,
            IOptions<ProxyOptions> proxyOptions,
            IHttpClientFactory httpClientFactory,
            ILogger<JoinWebinarProcessMiddleware> logger)
        {
            _urlHelper = urlHelper;
            _next = next;
            _checksumHelper = checksumHelper;
            _proxyOptions = proxyOptions.Value;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _bbbOptions = bbbOptions.Value;
        }

        public async Task Invoke(HttpContext context, ILogger<JoinWebinarProcessMiddleware> logger)
        {
            string bbbUrl = BuildBigbluebuttonJoinUrl(context.Request.Query);
            logger.LogInformation("bbbUrl: {bbbUrl}", bbbUrl);

            if (string.IsNullOrEmpty(bbbUrl))
            {
                var redirectUrl = _urlHelper.BuildErrorUrl(_proxyOptions.DefaultFallbackUrl, ErrorCodes.MeetingUnavailable);

                context.Response.Redirect(redirectUrl);

                return;
            }

            // When we have a valid BigBlueButton server => continue to get cookies & url of the meeting from BigBlueButton server.
            JoinMeetingModel meetingSecretData = await GetOriginalMeetingInfo(bbbUrl);
            logger.LogInformation("meetingSecretDataUrl: {Url}", meetingSecretData.Url);

            if (meetingSecretData == null || meetingSecretData.Cookie == null)
            {
                var redirectUrl = _urlHelper.BuildErrorUrl(_proxyOptions.DefaultFallbackUrl, ErrorCodes.MeetingUnavailable);
                context.Response.Redirect(redirectUrl);
                return;
            }

            // Finally, set cookies for the current request and redirect the user to the meeting.
            context.Response.Cookies.Append(meetingSecretData.Cookie.Name, meetingSecretData.Cookie.Value, new CookieOptions { Path = "/", IsEssential = true });
            context.Response.Redirect(meetingSecretData.Url);
        }

        private string BuildBigbluebuttonJoinUrl(IQueryCollection query)
        {
            // Extract meeting url from the requested url.
            var joinMeetingQuery = _urlHelper.GetJoinMeetingQueryString(query);

            // Build checksum from the client meeting url.
            var joinMeetingChecksum = _checksumHelper.BuildChecksumFromQuery($"join{joinMeetingQuery}", _bbbOptions.BBBSecretKey);

            var bbbArea = query["area"];
            if (string.IsNullOrEmpty(bbbArea))
            {
                return string.Empty;
            }

            // Build BBB URL
            string bbbUrl = $"{_bbbOptions.WebinarAddress}/bbb-{bbbArea}/bigbluebutton/api/join?{joinMeetingQuery}&checksum={joinMeetingChecksum}";
            return bbbUrl;
        }

        private async Task<JoinMeetingModel> GetOriginalMeetingInfo(string bbbUrl)
        {
            var client = _httpClientFactory.CreateClient(HttpClientSchemaConstant.BigBlueButtonClient);
            var response = await client.GetAsync(bbbUrl);

            if (response.StatusCode != HttpStatusCode.Found)
            {
                _logger.LogWarning(
                    "Failed to get meeting URL from BBB. StatusCode: {StatusCode}, detail: {Content}",
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync());

                return null;
            }

            var meetingCookie = ExtractCookieFromHeader(response.Headers);

            var joinMeetingModel = new JoinMeetingModel
            {
                Url = response.Headers.Location.ToString(),
                Cookie = meetingCookie
            };

            return joinMeetingModel;
        }

        private Cookie ExtractCookieFromHeader(HttpResponseHeaders headers)
        {
            // Get set cookie header from list response headers.
            var cookieHeaders = headers.SingleOrDefault(header => header.Key == "Set-Cookie");

            // Since the cookie header contains only one value, so we get the cookie value as the first item.
            var cookieValue = cookieHeaders.Value?.FirstOrDefault();

            if (cookieValue == null || !cookieValue.Contains("="))
            {
                _logger.LogWarning("Failed to extract meeting cookie from BBB.");
                return null;
            }

            // extract cookie name and value from {key=value; httpOnly=true}
            var normalizedCookieHeader = cookieValue?.Split(';').FirstOrDefault();

            var parsedCookies = normalizedCookieHeader.Split('=');

            return new Cookie(parsedCookies[0], parsedCookies[1]);
        }
    }
}
