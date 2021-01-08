using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.Settings;
using cxPlatform.Core;

namespace cxOrganization.Domain.Security.AccessServices
{
    public interface IUserAccessService
    {
        UserAccessResult CheckReadUserAccess(IWorkContext workContext, UserEntity executorUser, int ownerId,
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

        Task<UserAccessResult> CheckReadUserAccessAsync(IWorkContext workContext, UserEntity executorUser, int ownerId,
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

        UserAccessResult CheckReadUserAccess(IWorkContext workContext, int ownerId,
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

        Task<UserAccessResult> CheckReadUserAccessAsync(IWorkContext workContext, int ownerId,
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

        UserAccessResult CheckEditUserAccess<T>(IWorkContext workContext, T editingUserDto,
            IUserMappingService userMappingService) where T : UserDtoBase;

        Task<UserAccessResult> CheckEditUserAccessAsync<T>(IWorkContext workContext, T editingUserDto,
            IUserMappingService userMappingService) where T : UserDtoBase;

        EditabilityAccessSettingElement GetAssignRolePermission(IWorkContext workContext);

        bool CheckCreateUserAccess<T>(IWorkContext workContext, T editingUserDto) where T : UserDtoBase;
        Task<bool> CheckCreateUserAccessAsync<T>(IWorkContext workContext, T editingUserDto) where T : UserDtoBase;
    }
}