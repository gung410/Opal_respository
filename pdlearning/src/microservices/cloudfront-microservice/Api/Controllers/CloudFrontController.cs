using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.CloudFront;
using Conexus.Opal.Microservice.CloudFront.Api.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Thunder.Platform.AspNetCore.Controllers;
using Thunder.Platform.Core.Context;

namespace Conexus.Opal.Microservice.CloudFront.Api.Controllers
{
    [Route("api/cloudfront")]
    public class CloudFrontController : ApplicationApiController
    {
        private readonly CloudFrontSettings _cloudFrontSettings;

        public CloudFrontController(IUserContext userContext, IOptions<CloudFrontSettings> cloudFrontSettings) : base(userContext)
        {
            _cloudFrontSettings = cloudFrontSettings.Value;
        }

        [HttpGet("getCookieInfo")]
        public Task<object> GetCookieInfo()
        {
            var policySummary = CreateCloudFrontCookiePolicy();
            return Task.FromResult<object>(new
            {
                // Policy.
                PolicyKey = policySummary.cookie.Policy.Key,
                PolicyValue = policySummary.cookie.Policy.Value,

                // Signature.
                SignatureKey = policySummary.cookie.Signature.Key,
                SignatureValue = policySummary.cookie.Signature.Value,

                // Key pair.
                KeyPairIdKey = policySummary.cookie.KeyPairId.Key,
                KeyPairIdValue = policySummary.cookie.KeyPairId.Value
            });
        }

        [HttpPost("signin")]
#pragma warning disable CA1054 // URI-like parameters should not be strings
        public IActionResult SetCookieInfo([FromQuery] string returnUrl)
#pragma warning restore CA1054 // URI-like parameters should not be strings
        {
            var policySummary = CreateCloudFrontCookiePolicy();
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = policySummary.expiresOn,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            HttpContext.Response.Cookies.Append(policySummary.cookie.Policy.Key, policySummary.cookie.Policy.Value, cookieOptions);
            HttpContext.Response.Cookies.Append(policySummary.cookie.Signature.Key, policySummary.cookie.Signature.Value, cookieOptions);
            HttpContext.Response.Cookies.Append(policySummary.cookie.KeyPairId.Key, policySummary.cookie.KeyPairId.Value, cookieOptions);

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Ok("Cookie signed in successfully!");
        }

        private (DateTime expiresOn, CookiesForCustomPolicy cookie) CreateCloudFrontCookiePolicy()
        {
            using (var reader = new StreamReader(_cloudFrontSettings.PrivateKey))
            {
                DateTime utcNow = DateTime.UtcNow;
                DateTime expiresOn = utcNow.Add(_cloudFrontSettings.CookieExpiration);
                DateTime activeFrom = utcNow.Add(_cloudFrontSettings.CookieValidStart);
                var resourceUrlOrPath = new Uri(new Uri(_cloudFrontSettings.CloudFrontUrl), "*").AbsoluteUri;
                var cookies = AmazonCloudFrontCookieSigner.GetCookiesForCustomPolicy(
                    resourceUrlOrPath,
                    reader,
                    _cloudFrontSettings.CloudFrontKeyPairId,
                    expiresOn,
                    activeFrom,
                    null);

                return (expiresOn, cookies);
            }
        }
    }
}
