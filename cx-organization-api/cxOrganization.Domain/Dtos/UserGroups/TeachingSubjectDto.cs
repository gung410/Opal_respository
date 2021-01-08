using cxOrganization.Client.UserGroups;
using cxPlatform.Client.ConexusBase;
using System;
using System.Collections.Generic;
using System.Text;

namespace cxOrganization.Domain.Dtos.UserGroups
{
    public class TeachingSubjectDto : UserGroupDtoBase
    {
        public int? DepartmentId { get; internal set; }

        public int? ArchetypeId { get; internal set; }

        public override int? GetParentDepartmentId()
        {
            return DepartmentId;
        }

        public override int? GetParentUserId()
        {
            throw new NotImplementedException();
        }
        public override void SetParentDepartmentId(int? parentDepartmentId)
        {
            DepartmentId = parentDepartmentId;
        }
    }
}
