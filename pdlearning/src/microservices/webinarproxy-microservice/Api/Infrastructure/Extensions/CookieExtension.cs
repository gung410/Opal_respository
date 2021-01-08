using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Microservice.WebinarProxy.Infrastructure.Extensions
{
    public static class CookieExtension
    {
        private static readonly string[] _groupObjectIds = new[] { "sid", "sub", "emails", "role" };

        public static void FilterGroupClaims(this CookieSigningInContext context)
        {
            var principal = context.Principal;
            if (principal.Identity is ClaimsIdentity identity)
            {
                var unused = identity.FindAll(GroupsToKeep).ToList();
                unused.ForEach(c => identity.TryRemoveClaim(c));
            }
        }

        private static bool GroupsToKeep(Claim claim)
        {
            return !_groupObjectIds.Contains(claim.Type);
        }
    }
}
