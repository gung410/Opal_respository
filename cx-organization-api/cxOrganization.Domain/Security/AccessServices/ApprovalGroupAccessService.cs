using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.Security.AccessServices
{

    public class ApprovalGroupAccessService : UserGroupAccessServiceBase, IApprovalGroupAccessService
    {
        private readonly AccessSettings _accessSettings;

        public ApprovalGroupAccessService(ILogger<ApprovalGroupAccessService> logger,
            IUserRepository userRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IOptions<AccessSettings> accessSettingsOption,
            IDTDEntityRepository dtdEntityRepository,
            IDepartmentTypeRepository departmentTypeRepository) :
            base(logger, userRepository, hierarchyDepartmentRepository, dtdEntityRepository, departmentTypeRepository)
        {
            _accessSettings = accessSettingsOption.Value;
        }

        public AccessStatus CheckReadApprovalGroupAccess(IWorkContext workContext,
            ref List<int> userIds,
            ref List<int> parentDepartmentIds)
        {
            if (_accessSettings != null && !_accessSettings.DisableReadApprovalGroupAccessChecking)
            {
                var accessResult = CheckUserGroupAccessInternal(workContext, _accessSettings.ReadApprovalGroupAccess,
                    userIds, parentDepartmentIds );
                var executorUser = accessResult.ExecutorUser;
                if (accessResult.AccessStatus == AccessStatus.AccessDenied)
                {
                    _logger.LogWarning(
                        $"Logged-in user with extId {executorUser.ExtId} (id {executorUser.UserId}) does not have access on approval group");
                }

                return accessResult.AccessStatus;
            }

            return AccessStatus.AccessGranted;
        }
        public async Task<(AccessStatus AccessStatus, List<int> UserIds, List<int> ParentDepartmentIds)> CheckReadApprovalGroupAccessAsync(
            IWorkContext workContext,
            List<int> userIds,
            List<int> parentDepartmentIds)
        {
            if (_accessSettings != null && !_accessSettings.DisableReadApprovalGroupAccessChecking)
            {
                var userGroupAccess = await CheckUserGroupAccessInternalAsync(workContext, _accessSettings.ReadApprovalGroupAccess,
                    userIds, parentDepartmentIds);
                userIds = userGroupAccess.UserIds;
                parentDepartmentIds = userGroupAccess.ParentDepartmentIds;

                if (userGroupAccess.AccessStatus == AccessStatus.AccessDenied)
                {
                    _logger.LogWarning(
                        $"Logged-in user with extId {userGroupAccess.ExecutorUser?.ExtId} (id {userGroupAccess.ExecutorUser?.UserId}) does not have access on approval group");
                    return (userGroupAccess.AccessStatus, userIds, parentDepartmentIds);
                }
            }

            return (AccessStatus.AccessGranted, userIds, parentDepartmentIds);

        }
    }
}