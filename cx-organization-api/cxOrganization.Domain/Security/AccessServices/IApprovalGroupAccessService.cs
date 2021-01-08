using System.Collections.Generic;
using System.Threading.Tasks;
using cxPlatform.Core;

namespace cxOrganization.Domain.Security.AccessServices
{
    public interface IApprovalGroupAccessService
    {
        AccessStatus CheckReadApprovalGroupAccess(IWorkContext workContext,
            ref List<int> userIds,
            ref List<int> parentDepartmentIds);

        Task<(AccessStatus AccessStatus, List<int> UserIds, List<int> ParentDepartmentIds)> CheckReadApprovalGroupAccessAsync(
            IWorkContext workContext,
            List<int> userIds,
            List<int> parentDepartmentIds);
    }
}