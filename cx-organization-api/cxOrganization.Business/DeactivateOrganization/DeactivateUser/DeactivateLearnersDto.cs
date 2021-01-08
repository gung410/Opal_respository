using System.Collections.Generic;
using cxOrganization.Business.Common;
using cxOrganization.Business.Validations;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.DeactivateOrganization.DeactivateUser
{
    public class DeactivateLearnersDto : DeactivateUsersDto
    {
        /// <summary>
        /// A list of learner identities who will be deactivated
        /// </summary>

        [IdentityValidate(Required = true)]
        [ArchetypeIdentityValidate(ArchetypeEnum.Learner)]
        public override List<IdentityWithClaimDto> Identities { get; set; }

    }
}