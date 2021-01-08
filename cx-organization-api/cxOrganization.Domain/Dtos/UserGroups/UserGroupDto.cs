using cxOrganization.Client.UserGroups;
using cxPlatform.Client.ConexusBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Dtos.UserGroups
{
    public class UserGroupDto : UserGroupDtoBase
    {
        public int? DepartmentId { get; internal set; }

        //Keep backware compatibility 
        public long? UserId => UserIdentity?.Id;
       

        public IdentityDto UserIdentity { get; internal set; }

        public override int? GetParentDepartmentId()
        {
            return DepartmentId;
        }

        public override int? GetParentUserId()
        {
            return (int?)UserId;
        }
        public override void SetParentDepartmentId(int? parentDepartmentId)
        {
            DepartmentId = parentDepartmentId;
        }
    }
}
