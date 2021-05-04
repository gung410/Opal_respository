using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Domain.AdvancedWorkContext;
using cxPlatform.Core;

namespace cxOrganization.Domain.Security.AccessServices
{
    public interface IApprovalGroupAccessService
    {
        AccessStatus CheckReadApprovalGroupAccess(IAdvancedWorkContext workContext,
            ref List<int> userIds,
            ref List<int> parentDepartmentIds);

        Task<(AccessStatus AccessStatus, List<int> UserIds, List<int> ParentDepartmentIds)> CheckReadApprovalGroupAccessAsync(
            IAdvancedWorkContext workContext,
            List<int> userIds,
            List<int> parentDepartmentIds);
    }
}