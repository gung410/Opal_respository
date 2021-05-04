using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Repositories.QueryBuilders;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using cxOrganization.Domain.Services.Reports;
using Gender = cxOrganization.Domain.Enums.Gender;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.AdvancedWorkContext;

namespace cxOrganization.Domain.Repositories
{
    /// <summary>
    /// Interface IUserRepository
    /// </summary>
    public interface IUserRepository : IRepository<UserEntity>
    {
        List<UserEntity> GetUserByIds(List<int> userIds,
            bool includeUserType = false,
            bool includeUserGroup = false,
            bool includeDepartment = false);
        PaginatedList<UserEntity> GetUsers(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> extIds = null,
            List<string> ssnList = null,
            List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<AgeRange> ageRanges = null,
            List<Gender> genders = null,
            List<EntityStatusEnum> memberStatuses = null,
            DateTime? memberValidFromBefore = null,
            DateTime? memberValidFromAfter = null,
            DateTime? memberValidToBefore = null,
            DateTime? memberValidToAfter = null,
            string searchKey = null,
            List<string> loginServiceClaims = null,
            List<string> loginServiceClaimTypes = null,
            List<int> loginServiceIds = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            IncludeDepartmentOption includeDepartment = IncludeDepartmentOption.None,
            bool includeLoginServiceUsers = false,
            IncludeUserTypeOption includeUserTypes = IncludeUserTypeOption.None,
            bool filterOnParentHd = true,
            IncludeUgMemberOption includeUGMembers = IncludeUgMemberOption.None,
            List<string> jsonDynamicData = null,
            bool? externallyMastered = null,
            bool skipPaging = false,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            List<int> orgUnittypeIds = null,
            List<List<int>> multiUserTypeFilters = null,
            List<List<int>> multiUserGroupFilters = null,
            List<List<string>> multiUserTypeExtIdFilters = null,
            List<string> emails = null,
            bool forUpdating = false,
            bool includeOwnUserGroups = false,
            DateTime? entityActiveDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            bool filterOnUd = false,
            List<int> exceptUserIds = null);
        Task<PaginatedList<UserEntity>> GetAllUsers(int pageIndex, int pageSize);
        Task<PaginatedList<UserEntity>> GetUsersAsync(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> departmentExtIds = null,
            List<string> extIds = null,
            List<string> ssnList = null,
            List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<AgeRange> ageRanges = null,
            List<Gender> genders = null,
            List<EntityStatusEnum> memberStatuses = null,
            DateTime? memberValidFromBefore = null,
            DateTime? memberValidFromAfter = null,
            DateTime? memberValidToBefore = null,
            DateTime? memberValidToAfter = null,
            string searchKey = null,
            List<string> loginServiceClaims = null,
            List<string> loginServiceClaimTypes = null,
            List<int> loginServiceIds = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            IncludeDepartmentOption includeDepartment = IncludeDepartmentOption.None,
            bool includeLoginServiceUsers = false,
            IncludeUserTypeOption includeUserTypes = IncludeUserTypeOption.None,
            bool filterOnParentHd = true,
            IncludeUgMemberOption includeUGMembers = IncludeUgMemberOption.None,
            List<string> jsonDynamicData = null,
            bool? externallyMastered = null,
            bool skipPaging = false,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            List<int> orgUnittypeIds = null,
            List<List<int>> multiUserTypeFilters = null,
            List<List<int>> multiUserGroupFilters = null,
            List<List<string>> multiUserTypeExtIdFilters = null,
            List<string> emails = null,
            bool forUpdating = false,
            bool includeOwnUserGroups = false,
            DateTime? entityActiveDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            bool filterOnUd = false,
            List<int> exceptUserIds = null,
            int? currentDepartmentIdForSorting = null);

        PaginatedList<UserEntity> SearchActors(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> extIds = null,
            List<string> ssnList = null,
            List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<AgeRange> ageRanges = null,
            List<Gender> genders = null,
            List<EntityStatusEnum> memberStatuses = null,
            DateTime? memberValidFromBefore = null,
            DateTime? memberValidFromAfter = null,
            DateTime? memberValidToBefore = null,
            DateTime? memberValidToAfter = null,
            string searchKey = null,
            List<string> loginServiceClaims = null,
            List<string> loginServiceClaimTypes = null,
            List<int> loginServiceIds = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            bool includeDepartment = false,
            bool includeLoginServiceUsers = false,
            bool includeUserTypes = false,
            List<int> exceptUserIds = null);
        List<UserEntity> GetUserByIds(List<int> userIds, Expression<Func<UserEntity, object>>[] includeProperties);
        UserEntity Insert(UserEntity entity, int useHashPassword, bool useOTP, int defaultHashMethod);
        UserEntity Update(UserEntity entity,
            int useHashPassword,
            bool useOTP,
            int defaultHashMethod,
            bool changePassword = false,
            bool generateRandomPassword = true,
            string updatedPassword = "");
        UserEntity GetUserById(int userId);
        Task<UserEntity> GetUserAsync(int ownerId, int? userId, string extId);

        List<UserEntity> GetUsersByDepartmentIds(List<int> departmentIds, params EntityStatusEnum[] filters);
        List<UserEntity> GetUsersByDepartmentIdIncludeUserTypesUserGroup(int departmentId, int ownerId, params EntityStatusEnum[] filters);
        List<UserEntity> GetUsersByUserGroupIds(List<int> userGroupIds,
            List<int> usertypeIds,
            List<int> gender = null,
            List<string> ages = null,
            string courtry = "");
        List<UserEntity> GetUsersByUserIdsAndArchetypeIds(List<long?> userIds, List<int> allowArchetypeIds);
        UserEntity GetUserByUserExtId(string userExtId, int customerId = 0);
        UserEntity GetUserBySSN(string ssn, bool includeDepartment = false, int customerId = 0);
        List<UserEntity> GetUsersByUserExtId(string extId);
        UserEntity GetDefaultUserByCustomer(int customerId);
        List<UserEntity> SearchUser(string searchKey, int departmentId, int hdId = 0, bool deepSearch = false, int maxTake = 0);
        List<UserEntity> SearchUser(string firstSearchKey, string secondSearchKey, int departmentId, int hdId = 0, bool deepSearch = false, int maxTake = 0);
        List<UserEntity> GetUsersIncludeUserTypes(List<int> userIds, bool includeDepartment = false);
        void AddRemoveUserTypes(UserEntity user, List<int> userTypeIdsToAdd, List<int> userTypeIdsToRemove);
        List<UserEntity> GetUsersByDepartmentIdsAndUserTypeIds(List<int> departmentIds, List<int> userTypeIds);
        List<UserEntity> GetUsers(int ownerId, int? userId, string username, string extId);
        Task<List<UserEntity>> GetUsersAsync(int ownerId, int? userId, string username, string extId);
        List<UserEntity> GetUsersByUsername(int ownerId, string username, params EntityStatusEnum[] filters);
        IEnumerable<UserEntity> GetUsersByDepartment(int departmentId, bool includeDepartment = true, bool includeUserType = true, bool includeUserGroup = false, bool putToCache = false, bool allowGetUserDeleted = false);
        List<UserEntity> GetListUserByDepartmentIds(IEnumerable<int> departmentIds, bool includeLinkedUsers);
        bool CheckUsername(int ownerId, int userId, string userName);
        UserEntity GetUserForUpdateInsert(int userId);
        List<UserEntity> GetUsersByUsernameForUpdate(int ownerId, string username, params EntityStatusEnum[] filters);
        UserEntity GetUserIncludeDepartmentIncludeUserTypesIncludeUserGroups(int userId, bool putToCache = true);
        List<UserEntity> GetUsersByExtIds(int customerId,
            int ownerId,
            List<string> userExtIds,
            bool includeUserType = false,
            bool includeUserGroup = false,
            bool includeDepartment = false,
            bool includeUgMember = false,
            bool includeUserGroupFromUgMember = false);

        int CountUsers(int ownerId = 0,
          List<int> customerIds = null,
          List<int> userIds = null,
          List<int> userGroupIds = null,
          List<EntityStatusEnum> statusIds = null,
          List<ArchetypeEnum> archetypeIds = null,
          List<int> userTypeIds = null,
          List<string> userTypeExtIds = null,
          List<int> parentDepartmentIds = null,
          List<string> extIds = null,
          List<string> ssnList = null,
          List<string> userNames = null,
          DateTime? lastUpdatedBefore = null,
          DateTime? lastUpdatedAfter = null,
          List<AgeRange> ageRanges = null,
          List<Gender> genders = null,
          bool filterOnParentHd = true,
          List<string> jsonDynamicData = null,
          DateTime? createdAfter = null,
          DateTime? createdBefore = null,
          bool? externallyMastered = null,
          bool filterOnUd = false,
          List<int> exceptUserIds = null);

        Task<int> CountUsersAsync(int ownerId = 0,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> extIds = null,
            List<string> ssnList = null,
            List<string> userNames = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            List<AgeRange> ageRanges = null,
            List<Gender> genders = null,
            bool filterOnParentHd = true,
            List<string> jsonDynamicData = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            bool? externallyMastered = null,
            bool filterOnUd = false,
            List<int> exceptUserIds = null);
        IDictionary<int, int> CountUsersGroupByDepartment(int ownerId = 0,
         List<int> customerIds = null,
         List<int> userIds = null,
         List<int> userGroupIds = null,
         List<EntityStatusEnum> statusIds = null,
         List<ArchetypeEnum> archetypeIds = null,
         List<int> userTypeIds = null,
         List<string> userTypeExtIds = null,
         List<int> parentDepartmentIds = null,
         List<string> userExtIds = null,
         List<string> ssnList = null,
         List<string> userNames = null,
         DateTime? lastUpdatedBefore = null,
         DateTime? lastUpdatedAfter = null,
         List<AgeRange> ageRanges = null,
         List<Gender> genders = null,
         bool filterOnParentHd = true,
         bool filterOnUd = false,
         List<int> exceptUserIds = null);
        List<string> GetModifiedProperties(UserEntity entity);
        Task<Dictionary<int, int>> CountUsersGroupByDepartmentAsync(int ownerId = 0,
         List<int> customerIds = null,
         List<int> userIds = null,
         List<int> userGroupIds = null,
         List<EntityStatusEnum> statusIds = null,
         List<ArchetypeEnum> archetypeIds = null,
         List<int> userTypeIds = null,
         List<string> userTypeExtIds = null,
         List<int> parentDepartmentIds = null,
         List<string> userExtIds = null,
         List<string> ssnList = null,
         List<string> userNames = null,
         DateTime? lastUpdatedBefore = null,
         DateTime? lastUpdatedAfter = null,
         List<AgeRange> ageRanges = null,
         List<Gender> genders = null,
         bool filterOnParentHd = true,
         bool filterOnUd = false,
         List<int> exceptUserIds = null);

        Task<UserEntity> GetOrSetUserFromWorkContext(IAdvancedWorkContext workContext);
        Task<IList<UserRole>> GetOrSetUserRoleFromWorkContext(IAdvancedWorkContext workContext);

        IQueryable<UserEntity> GetQueryAsNoTracking(params EntityStatusEnum[] entityStatus);

        IQueryable<UserEntity> GetQueryIncludeDeletedUsers();

        Task<List<UserAccountStatisticsInfo>> GetUserStatisticsInfos(int ownerId, int customerId, List<EntityStatusEnum> entityStatuses,
            DateTime? createdAfter, DateTime? createdBefore);

        Task<(List<CountUserEntity> UserCountValues, int TotalUser)> CountUserGroupByAsync(int ownerId,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<int> departmentIds = null,
            List<string> extIds = null,
            List<string> jsonDynamicData = null,
            List<int> exceptUserIds = null,
            List<List<int>> multiUserTypeFilters = null,
            List<List<int>> multiUserGroupFilters = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
            UserGroupByField groupByField = UserGroupByField.None);
        List<UserEntity> GetUserForMigratingSsn(int pageNo, int pageSize);
        List<UserEntity> GetUserForFixingSSN(int pageNo, int pageSize);

        List<UserEntity> GetUserForUpdateSsnHash(int pageNo, int pageSize);
    }
    
}
