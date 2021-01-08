using System.Collections.Generic;
using cxOrganization.Business.Common;
using cxOrganization.Business.Validations;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.DeactivateOrganization.DeactivateUser
{
    public class DeactivateEmployeesDto : DeactivateUsersDto
    {
        /// <summary>
        /// A list of employee identities who will be deactivated
        /// </summary>

        [IdentityValidate(Required = true)]
        [ArchetypeIdentityValidate(ArchetypeEnum.Employee)]
        public override List<IdentityWithClaimDto> Identities { get; set; }

    }
}