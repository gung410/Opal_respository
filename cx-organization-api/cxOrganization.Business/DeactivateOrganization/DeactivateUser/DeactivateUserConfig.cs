using System.Collections.Generic;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Business.DeactivateOrganization.DeactivateUser
{
    public class DeactivateUserConfig
    {
        public List<string> DeactivateUserByRoles { get; set; }
        public List<EntityStatusEnum> AcceptedStatusesForDeactivating { get; set; }

    }
}
