using cxOrganization.Client.Departments;
using cxOrganization.Domain.Entities;

namespace cxOrganization.Domain.Mappings
{
    public interface IHierarchyDepartmentMappingService
    {
        HierachyDepartmentIdentityDto ToDto(HierarchyDepartmentEntity value, HierarchyDepartmentEntity parentHd = null, int? childrenCount = null, bool getDetailDepartment = true,
            bool includeDepartmentType = false);
    }
}