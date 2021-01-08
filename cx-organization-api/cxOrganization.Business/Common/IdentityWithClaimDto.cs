using System.Collections.Generic;
using System.Linq;
using System.Text;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.Common
{
    public class IdentityWithClaimDto : IdentityDto
    {
        public Dictionary<string, string> Claims { get; set; }

        public override string ToString()
        {
            return ToStringInfo();
        }

        public string ToStringInfo(bool includeOwnerCustomer=true)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0} with", Archetype);
            if (Id > 0)
                stringBuilder.AppendFormat(" id {0},", Id);
            if (!string.IsNullOrEmpty(ExtId))
                stringBuilder.AppendFormat(" extId {0},", ExtId);
            if (Claims != null && Claims.Count > 0)
            {
                stringBuilder.AppendFormat(" claim value");

                foreach (var claim in Claims)
                {
                    stringBuilder.AppendFormat(" {0}(type {1}),", claim.Value, claim.Key);
                }
            }
            return includeOwnerCustomer 
                ? string.Format("{0} in customer {1}, owner {2}", stringBuilder.ToString().TrimEnd(','), OwnerId, CustomerId) 
                : stringBuilder.ToString().TrimEnd(',');
        }
        public bool Match(IdentityWithClaimDto otherIdentityWithClaimDto)
        {
            return this.OwnerId == otherIdentityWithClaimDto.OwnerId
                   && this.CustomerId == otherIdentityWithClaimDto.CustomerId
                   && this.Archetype == otherIdentityWithClaimDto.Archetype
                   && (this.Id ?? 0) == (otherIdentityWithClaimDto.Id ?? 0)
                   && (this.ExtId ?? string.Empty) == (otherIdentityWithClaimDto.ExtId ?? string.Empty)
                   && MatchClaims(otherIdentityWithClaimDto.Claims);
        }

        private bool MatchClaims(Dictionary<string, string> otherClaims)
        {
            var sourceClaims = (Claims == null ? new Dictionary<string, string>() :
                new Dictionary<string, string>(Claims)).OrderBy(k=>k.Key).ToList();
            var targetClaims = (otherClaims == null ? new Dictionary<string, string>() 
                : new Dictionary<string, string>(otherClaims)).OrderBy(k => k.Key).ToList();
            if (sourceClaims.Count != targetClaims.Count) return false;
            for (int i = 0; i < sourceClaims.Count; i++)
            {
                var sourceClaim = sourceClaims[i];
                var targetClaim = targetClaims[i];

                if (sourceClaim.Key != targetClaim.Key || sourceClaim.Value != targetClaim.Value)
                    return false;
            }
            return true;


        }

        
    }
}