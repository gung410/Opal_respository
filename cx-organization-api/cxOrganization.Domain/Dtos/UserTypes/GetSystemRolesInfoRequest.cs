using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Dtos.UserTypes
{
    public class GetSystemRolesInfoRequest
    {
        public List<int> systemRoleIds { get; set; }
        public List<string> systemRoleExtIds { get; set; }
        public bool? includeLocalizedData { get; set; }
        public bool? includeSystemRolePermissionSubjects { get; set; }

        public GetSystemRolesInfoRequest(List<int> systemRoleIds, List<string> systemRoleExtIds, bool includeLocalizedData = false, bool includeSystemRolePermissionSubjects = false)
        {
            this.systemRoleExtIds = systemRoleExtIds;
            this.systemRoleIds = systemRoleIds;
            this.includeLocalizedData = includeLocalizedData;
            this.includeSystemRolePermissionSubjects = includeSystemRolePermissionSubjects;
        }
    }
}
