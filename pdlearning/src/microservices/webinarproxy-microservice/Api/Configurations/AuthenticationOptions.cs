using System.Collections.Generic;

namespace Microservice.WebinarProxy.Configurations
{
    public class AuthenticationOptions
    {
        public string OidcClientSecret { get; set; }

        public string OidcClientId { get; set; }

        public IList<string> ProxyAppScope { get; set; }

        public bool SecureCookieAlways { get; set; }

        public string IdmInternalUrl { get; set; }

        public string IdmPublicUrl { get; set; }

        public string LoginRedirectUrl { get; set; }

        public string LogoutRedirectUrl { get; set; }

        public string SigninOidcPath { get; set; }

        public string SignOutOidcPath { get; set; }

        public bool UseAuthenticatedForJsonRequest { get; set; }

        public string CheckSessionEndpoint { get; set; }

        public HashSet<string> IgnoreAuthenticationPaths { get; set; }
    }
}
