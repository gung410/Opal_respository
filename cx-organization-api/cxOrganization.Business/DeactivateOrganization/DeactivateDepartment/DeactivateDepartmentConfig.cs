using cxPlatform.Client.ConexusBase;
using System.Collections.Generic;

namespace cxOrganization.Business.DeactivateOrganization.DeactivateDepartment
{
    public class DeactivateDepartmentConfig
    {
        public DeactivateDepartmentConfig()
        {
            DeactivateIfContainingUsers = new DeactivateIfContainingUsersConfig();
        }
        public List<string> ExecutiveRoles { get; set; }
        public DeactivateIfContainingUsersConfig DeactivateIfContainingUsers { get; set; }
        public List<EntityStatusEnum> AcceptedStatusesForDeactivating { get; set; }
    }

    public class DeactivateIfContainingUsersConfig
    {
        public bool Enable { get; set; }

        public List<EntityStatusEnum> UserEntityStatuses { get; set; }
    }
}
