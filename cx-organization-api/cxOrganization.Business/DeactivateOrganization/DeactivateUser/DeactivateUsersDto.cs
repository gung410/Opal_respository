using System.Collections.Generic;
using cxOrganization.Business.Common;
using cxOrganization.Business.Validations;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.DeactivateOrganization.DeactivateUser
{
    public abstract class DeactivateUsersDto
    {
        public abstract List<IdentityWithClaimDto> Identities { get; set; }

        public bool SelfDeactivate { get; set; }
        /// <summary>
        /// Set true to deactivate login service data when deactivate user information
        /// </summary>
        public bool DeactivateLoginService { get; set; }

        /// <summary>
        /// Set true to deactivate membership between user and other data such as usergroup
        /// </summary>
        public bool DeactivateMembership { get; set; }

        /// <summary>
        /// List entity status that user belong to. Default will get from config.
        /// </summary>
        public List<EntityStatusEnum> AcceptedEntityStatuses { get; set; }

        /// <summary>
        /// A identity of user who execute deactivating. If user is self-deactivating, please set value same with Identities
        /// </summary>
        [IdentityValidate(Required = false)]
        [UserArchetypeIdentityValidate]
        public IdentityWithClaimDto UpdatedByIdentity { get; set; }
    }
}
