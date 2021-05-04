using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;

namespace cxOrganization.Domain.Security.AccessServices
{
    public interface IUserAccessService
    {
        UserAccessResult CheckReadUserAccess(IAdvancedWorkContext workContext, UserEntity executorUser, int ownerId,
            List<int> customerIds,
            List<string> userExtIds,
            List<string> loginServiceClaims,
            List<int> userIds,
            List<int> userGroupIds,
            List<int> parentDepartmentIds,
            List<List<int>> multiUserGroupFilters,
            List<int> userTypeIdsFilter,
            List<string> userTypeExtIdsFilter,
            List<List<int>> multipleUserTypeIdsFilter,
            List<List<string>> multipleUserTypeExtIdsFilter,
            string accessPolicy = null);

        Task<UserAccessResult> CheckReadUserAccessAsync(IAdvancedWorkContext workContext, UserEntity executorUser, int ownerId,
            List<int> customerIds,
            List<string> userExtIds,
            List<string> loginServiceClaims,
            List<int> userIds,
            List<int> userGroupIds,
            List<int> parentDepartmentIds,
            List<List<int>> multiUserGroupFilters,
            List<int> userTypeIdsFilter,
            List<string> userTypeExtIdsFilter,
            List<List<int>> multipleUserTypeIdsFilter,
            List<List<string>> multipleUserTypeExtIdsFilter,
            string accessPolicy = null);

        UserAccessResult CheckReadUserAccess(IAdvancedWorkContext workContext, int ownerId,
            List<int> customerIds,
            List<string> userExtIds,
            List<string> loginServiceClaims,
            List<int> userIds,
            List<int> userGroupIds,
            List<int> parentDepartmentIds,
            List<List<int>> multiUserGroupFilters,
            List<int> userTypeIdsFilter,
            List<string> userTypeExtIdsFilter,
            List<List<int>> multipleUserTypeIdsFilter,
            List<List<string>> multipleUserTypeExtIdsFilter,
            string accessPolicy = null);

        Task<UserAccessResult> CheckReadUserAccessAsync(IAdvancedWorkContext workContext, int ownerId,
            List<int> customerIds,
            List<string> userExtIds,
            List<string> loginServiceClaims,
            List<int> userIds,
            List<int> userGroupIds,
            List<int> parentDepartmentIds,
            List<List<int>> multiUserGroupFilters,
            List<int> userTypeIdsFilter,
            List<string> userTypeExtIdsFilter,
            List<List<int>> multipleUserTypeIdsFilter,
            List<List<string>> multipleUserTypeExtIdsFilter,
            string accessPolicy = null);

        UserAccessResult CheckEditUserAccess<T>(IAdvancedWorkContext workContext, T editingUserDto,
            IUserMappingService userMappingService) where T : UserDtoBase;

        Task<UserAccessResult> CheckEditUserAccessAsync<T>(IAdvancedWorkContext workContext, T editingUserDto,
            IUserMappingService userMappingService) where T : UserDtoBase;

        EditabilityAccessSettingElement GetAssignRolePermission(IAdvancedWorkContext workContext);

        bool CheckCreateUserAccess<T>(IAdvancedWorkContext workContext, T editingUserDto) where T : UserDtoBase;
        Task<bool> CheckCreateUserAccessAsync<T>(IAdvancedWorkContext workContext, T editingUserDto) where T : UserDtoBase;
    }
}