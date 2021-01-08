using System;

namespace Conexus.Opal.Microservice.CloudFront.Api.Settings
{
    public class CloudFrontSettings
    {
        public string CloudFrontKeyPairId { get; set; }

        public string PrivateKey { get; set; }

        public TimeSpan CookieExpiration { get; set; }

        public TimeSpan CookieValidStart { get; set; }

        public string CloudFrontUrl { get; set; }
    }
}
