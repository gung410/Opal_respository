using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using LearnerApp.Models;

namespace LearnerApp.Services.Identity
{
    public static class TokenAnalyzer
    {
        public static UserInfo Analyze(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = tokenHandler.ReadJwtToken(token);

            string SafeGetClaimValue(IEnumerable<Claim> claims, string claimType)
            {
                var claim = claims.FirstOrDefault(c => claimType.Equals(c.Type));
                return claim != null ? claim.Value : string.Empty;
            }

            var sub = SafeGetClaimValue(jwtToken.Claims, "sub");
            var name = SafeGetClaimValue(jwtToken.Claims, "name");
            var email = SafeGetClaimValue(jwtToken.Claims, "preferred_username");

            return new UserInfo { Sub = sub, Name = name, Email = email };
        }
    }
}
