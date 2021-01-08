using cxOrganization.Domain.Dtos.Users;
using System.Collections.Generic;
using System.Threading.Tasks;
using cxPlatform.Client.ConexusBase;

namespace cxOrganization.Domain.Services
{
    public interface IUserInfoService
    {
        Task<(int, UserInfoDto)> GetUserInfoAsync(string extId,
            bool includeBasicInfo = false,
            bool includeTagIds = true,
            bool includeUserTagGroups = false);

        /// <summary>
        /// Gets public user info which doesn't need to check permission on data.
        /// </summary>
        /// <param name="userCxIds">The list of user CxIds.</param>
        /// <returns></returns>
        Task<List<PublicUserInfo>> GetPublicUserInfoAsync(List<string> userCxIds);

        Task<PaginatedList<UserBasicInfo>> GetUserWithBasicInfos(List<int> userIds = null,
            List<string> extIds = null, List<string> emails = null, List<EntityStatusEnum> entityStatuses = null,
            string searchKey = null, List<int> departmentIds = null, bool? externallyMastered = null,
            List<int> userTypeIds = null, List<string> userTypeExtIds = null,
            List<List<int>> multiUserTypeFilters = null, List<List<string>> multiUserTypeExtIdFilters = null,
            int pageSize = 0, int pageIndex = 0, string orderBy = "", bool getFullIdentity = false, bool getEntityStatus = false, 
            List<int> userGroupIds= null,
            List<int> exceptUserIds = null, List<string> systemRolePermissions = null,
            string token = null);

        Task<PaginatedList<UserHierarchyInfo>> GetUserHierarchyInfos(List<int> userIds = null,
            List<string> extIds = null, List<string> emails = null, List<EntityStatusEnum> entityStatuses = null,
            List<int> departmentIds = null,
            int pageSize = 0, int pageIndex = 0, string orderBy = "");

        Task<List<UserCountByUserTypeDto>> GetUserCountByUserTypes(List<ArchetypeEnum> userTypeArchetypes, List<int> userGroupIds, List<int> userIds);
    }


}