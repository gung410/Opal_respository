using System.Collections.Generic;
using cxOrganization.Client;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Mappings
{
    public interface IDepartmentMappingService
    {
        List<int> DepartmentTypeIds { get; set; }
        ConexusBaseDto ToDepartmentDto(DepartmentEntity department, int parentDepartmentId, bool? getDynamicProperties = null);
        ConexusBaseDto ToDepartmentDto(DepartmentEntity department, bool? getDynamicProperties = null);
        DepartmentEntity ToDepartmentEntity(HierarchyDepartmentEntity parentHd, DepartmentEntity entity, DepartmentDtoBase department);
        IdentityStatusDto ToIdentityStatusDto(DepartmentEntity department);
        MemberDto ToMemberDto(DepartmentEntity department);
    }
}
