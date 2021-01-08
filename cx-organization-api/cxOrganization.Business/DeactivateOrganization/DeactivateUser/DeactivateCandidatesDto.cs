using System.Collections.Generic;
using cxOrganization.Business.Common;
using cxOrganization.Business.Validations;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.DeactivateOrganization.DeactivateUser
{
    public class DeactivateCandidatesDto:DeactivateUsersDto
    {
        /// <summary>
        /// A list of candidate identities who will be deactivated
        /// </summary>
        [IdentityValidate(Required = true)]
        [ArchetypeIdentityValidate(ArchetypeEnum.Candidate)]
        public override List<IdentityWithClaimDto> Identities { get; set; }
     
    }
}