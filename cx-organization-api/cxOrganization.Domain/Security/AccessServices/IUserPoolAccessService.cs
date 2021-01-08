using System.Collections.Generic;
using System.Threading.Tasks;
using cxPlatform.Core;

namespace cxOrganization.Domain.Security.AccessServices
{
    public interface IUserPoolAccessService
    {
        Task<(AccessStatus AccessStatus, List<int> UserIds, List<int> ParentDepartmentIds)> CheckReadUserPoolAccess(
            IWorkContext workContext,
            List<int> userIds,
            List<int> parentDepartmentIds);
    }
}