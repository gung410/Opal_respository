using cxOrganization.Client.Departments;
using cxOrganization.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Security.HierarchyDepartment
{
    public interface IHierarchyDepartmentPermissionService
    {
        ICollection<HierarchyDepartmentEntity> SecurityCheck(ICollection<HierarchyDepartmentEntity> hierarchyDepartmentEntities);
        bool UserIsAuthenticatedByToken();
        UserEntity GetWorkContextUser();
        int GetRootDepartmentId();
        Task<bool> HasFullAccessOnHierarchyDepartmentAsync();
        List<HierachyDepartmentIdentityDto> ProcessRemovingTheRootDepartment(ICollection<HierachyDepartmentIdentityDto> hierachyDepartmentIdentityDtos);
        Task<UserEntity> GetWorkContextUserAsync();
        Task<bool> IgnoreSecurityCheckAsync();
        Task<ICollection<HierarchyDepartmentEntity>> SecurityCheckAsync(ICollection<HierarchyDepartmentEntity> hierarchyDepartmentEntities);
    }
}