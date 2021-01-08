using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.Entities;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Interface IUserGroupRepository
    /// </summary>
    public interface IUserGroupRepository : IRepository<UserGroupEntity>
    {
        List<UserGroupEntity> GetUserGroupByIds(List<int> userGroupIds,
            List<int> allowArchetypeIds,
            bool includeDepartmenttype);
        PaginatedList<UserGroupEntity> GetUserGroups(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userGroupIds = null,
            List<int> memberUserIds = null,
            List<int> parentDepartmentIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<EntityStatusEnum> userStatusIds = null,
            List<GrouptypeEnum> groupTypes = null,
            List<int> archetypeIds = null,
            List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<int> parentUserIds = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            List<string> referrerTokens = null,
            List<string> referrerResources = null,
            List<ArchetypeEnum> referrerArchetypes = null,
            List<ArchetypeEnum> parentDepartmentArchetypes = null,
            List<ArchetypeEnum> parentUserArchetypes = null,
            string searchKey = null,
            bool includeDepartment = false,
            bool includeUser = false);

        Task<PaginatedList<UserGroupEntity>> GetUserGroupsAsync(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userGroupIds = null,
            List<int> memberUserIds = null,
            List<int> parentDepartmentIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<EntityStatusEnum> userStatusIds = null,
            List<GrouptypeEnum> groupTypes = null,
            List<int> archetypeIds = null,
            List<string> extIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<int> parentUserIds = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            List<string> referrerTokens = null,
            List<string> referrerResources = null,
            List<ArchetypeEnum> referrerArchetypes = null,
            List<ArchetypeEnum> parentDepartmentArchetypes = null,
            List<ArchetypeEnum> parentUserArchetypes = null,
            string searchKey = null,
            bool includeDepartment = false,
            bool includeUser = false);
        UserGroupEntity GetUserGroupByExtId(string extId, int? departmentId = null, int? customerId = null);
        List<UserGroupEntity> GetUserGroupByIds(List<int> userGroupIds, 
            List<int> allowArchetypeIds,
            Expression<Func<UserGroupEntity,
                object>>[] includeProperties,
            params EntityStatusEnum[] filters);
        List<UserGroupEntity> GetUserGroupByDepartmentIds(List<int> departmentIds, int? groupType = null, params EntityStatusEnum[] filters);
        List<UserGroupEntity> GetUserGroupsByDepartmentId(int departmentId, bool includeDeppartmentType = false, params EntityStatusEnum[] filters);
        List<UserGroupEntity> GetuserGroupsByDepartmentIdAndUserGroupType(int departmentId,
            int userGroupTypeId,
            bool isIncludeUsers = false,
            int departmentTypeId = 0,
            params EntityStatusEnum[] filters);
        UserGroupEntity GetUserGroupIncludeDepartmentType(int userGroupId, params EntityStatusEnum[] filters);
        List<UserGroupEntity> GetUserGroups(int? userGroupId, string extId, params EntityStatusEnum[] filters);
        List<UserGroupEntity> GetUserGroupByIdsIncludeProperties(List<int> userGroupIds,
            Expression<Func<UserGroupEntity,
                object>>[] includeProperties,
            params EntityStatusEnum[] filters);
        UserGroupEntity GetUserGroupById(int Id, bool includeUsers = true, bool includeUserType = false, params EntityStatusEnum[] filters);
        //bool UpdateUG_U(string userGroupIds, string userIds, List<int> userGroupIdsToRemove);

        List<UserGroupEntity> GetUserGroupsWithoutPaging(int ownerId = 0,
            List<int> customerIds = null,
            List<ArchetypeEnum> userGroupArchetypeIds = null,
            List<int> userGroupIds = null,
            List<int> parentDepartmentIds = null,
            List<string> userGroupExtIds = null,
            List<string> referrerTokens = null,
            List<string> referrerResources = null,
            List<ArchetypeEnum> referrerArchetypes = null,
            List<GrouptypeEnum> userGroupTypeIds = null,
            List<EntityStatusEnum> userGroupStatuses = null,
            List<string> referercxTokens = null,
            bool includeMemberUsers = false,
            List<int> parentUserIds = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<ArchetypeEnum> parentDepartmentArchetypes = null,
            List<ArchetypeEnum> parentUserArchetypes = null,
            List<int> memberUserIds = null,
            List<EntityStatusEnum> memberStatuses = null,
            bool includeUser = true,
            bool includeDepartment = true);

        List<UserGroupEntity> GetUserGroupsByArchetypeIds(List<ArchetypeEnum> archetypeIds);

        List<string> GetModifiedProperties(UserGroupEntity entity);

    }
}
