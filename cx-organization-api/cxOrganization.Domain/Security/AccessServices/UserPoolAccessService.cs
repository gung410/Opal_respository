using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.Security.AccessServices
{
    public class UserPoolAccessService : UserGroupAccessServiceBase, IUserPoolAccessService
    {
        private readonly AccessSettings _accessSettings;

        public UserPoolAccessService(ILogger<UserPoolAccessService> logger,
            IUserRepository userRepository,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository, IDTDEntityRepository dtdEntityRepository,
            IOptions<AccessSettings> accessSettingsOption,
            IDepartmentTypeRepository departmentTypeRepository) :
            base(logger, userRepository, hierarchyDepartmentRepository, dtdEntityRepository, departmentTypeRepository)
        {
            _accessSettings = accessSettingsOption.Value;
        }

        public async Task<(AccessStatus AccessStatus, List<int> UserIds, List<int> ParentDepartmentIds)> CheckReadUserPoolAccess(
            IWorkContext workContext,
            List<int> userIds,
            List<int> parentDepartmentIds)
        {
            if (_accessSettings != null && !_accessSettings.DisableReadUserPoolAccessChecking)
            {
                var userGroupAccess = await CheckUserGroupAccessInternalAsync(workContext, _accessSettings.ReadUserPoolAccess,
                    userIds, parentDepartmentIds);
                userIds = userGroupAccess.UserIds;
                parentDepartmentIds = userGroupAccess.ParentDepartmentIds;

                if (userGroupAccess.AccessStatus == AccessStatus.AccessDenied)
                {
                    _logger.LogWarning(
                        $"Logged-in user with extId {userGroupAccess.ExecutorUser?.ExtId} (id {userGroupAccess.ExecutorUser?.UserId}) does not have access on user pool");
                }
                return (userGroupAccess.AccessStatus, userIds, parentDepartmentIds);

            }

            return (AccessStatus.AccessGranted, userIds, parentDepartmentIds);

        }
    }
}