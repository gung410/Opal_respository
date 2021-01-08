using System.Collections.Generic;
using cxOrganization.Client.Departments;
using cxPlatform.Core;
using System.Threading.Tasks;
using cxOrganization.Domain.Dtos.Departments;

namespace cxOrganization.Domain.Security.AccessServices
{
    public interface IDepartmentAccessService
    {
        /// <summary>
        /// Gets the top hierarchy department which the currently logged-in user has access to.
        /// e.g: DLC could be in Branch level but he should be allowed to access his closet Division.
        /// </summary>
        /// <param name="workContext"></param>
        /// <returns></returns>
        Task<(HierachyDepartmentIdentityDto TopHierachyDepartmentIdentity, List<HierarchyInfo> AccessibleHierarchyInfos)> GetTopHierarchyDepartmentsByWorkContext(IWorkContext workContext);
    }
}