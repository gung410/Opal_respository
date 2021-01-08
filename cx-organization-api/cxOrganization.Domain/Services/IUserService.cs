using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using cxOrganization.Client;
using cxOrganization.Client.Account;
using cxOrganization.Client.Departments;
using cxOrganization.Domain.DomainEnums;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Settings;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;

namespace cxOrganization.Domain.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Insert user from user dto
        /// </summary>
        /// <param name="hierarchyDepartmentValidationDto"></param>
        /// <param name="userDto"></param>
        /// <returns></returns>
        ConexusBaseDto InsertUser(HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto, UserDtoBase userDto, IWorkContext workContext = null, bool isInsertedByImport = false);
        /// <summary>
        /// Update user from user dto
        /// </summary>
        /// <param name="hierarchyDepartmentValidationDto"></param>
        /// <param name="userDto"></param>
        /// <returns></returns>
        ConexusBaseDto UpdateUser(
            HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto,
            UserDtoBase userDto,
            bool skipCheckingEntityVersion = false,
            IWorkContext workContext = null,
            bool? isAutoArchived = null);
        List<UserDtoBase> GetUsers(HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto, bool includeSubUsers = false);
        /// <summary>
        /// Get userdto
        /// </summary>
        /// <param name="hierarchyDepartmentValidationDto"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        ConexusBaseDto GetUser(HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto, int userId);
        /// <summary>
        /// Get userdto
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        ConexusBaseDto GetUser(int userId);

        Task ProcessAutoArchiveUser(IWorkContext workContext);
        List<MemberDto> GetUserMemberships(HierarchyDepartmentValidationSpecification hierarchyDepartmentValidationDto, int userId, ArchetypeEnum userArcheType, ArchetypeEnum useTypeArcheType, ArchetypeEnum departmentTypeArcheType);
        List<MemberDto> GetUserMemberships(int userId,
            ArchetypeEnum userArcheType,
            List<ArchetypeEnum> membershipsArchetypeIds = null,
            List<EntityStatusEnum> membershipStatusIds = null,
            List<int> membershipIds = null,
            List<string> membershipExtIds = null);
        List<IdentityStatusDto> UpdateUserIdentifiers(List<IdentityStatusDto> userIdentities, List<int> allowArchetypeIds, string hdPath);

        Task<(List<HierachyDepartmentIdentityDto> HierachyDepartmentIdentities, bool AccessDenied)> GetUserHierarchyDepartmentIdentitiesAsync(int ownerId,
            int? userId, string userExtId, bool includeParentHDs = true, bool includeChildrenHDs = false,
            List<int> customerIds = null, List<EntityStatusEnum> departmentEntityStatuses = null,
            int? maxChildrenLevel = null, bool countChildren = false,
            List<int> departmentTypeIds = null, string departmentName = null, bool includeDepartmentType = false,
            bool getParentNode = false,
            bool countUser = false, List<EntityStatusEnum> countUserEntityStatuses = null,
            List<string> jsonDynamicData = null);

        Task<PaginatedList<UserEntity>> GetAllUsers(int pageIndex);

        /// <summary>
        /// Get User HierachyDepartment Identities
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="customerId">0 mean skip customer filtering</param>
        /// <returns></returns>
        List<HierachyDepartmentIdentityDto> GetUserHierachyDepartmentIdentitiesByExtId(string extId, int customerId = 0);
        Task<PaginatedList<UserWithIdpInfoDto>> GetUsersWithIdpInfoAsync(string searchKey, int pageIndex, int pageSize, bool includeUGMembers, bool includeDepartment, bool getRoles, List<string> loginServiceClaims, string orderBy, List<string> jsonDynamicData, bool? externallyMastered);

        /// <summary>
        /// Get User HierachyDepartment Identities
        /// </summary>
        /// <param name="userSsn"></param>
        /// <param name="customerId">0 mean skip customer filtering</param>
        /// <returns></returns>
        List<HierachyDepartmentIdentityDto> GetUserHierachyDepartmentIdentitiesBySSN(string userSsn, int customerId = 0);

        /// <summary>
        /// Get User IdentityStatus
        /// </summary>
        /// <param name="extId"></param>
        /// <param name="customerId">0 mean skip customer filtering</param>
        /// <returns></returns>
        IdentityStatusDto GetUserIdentityStatusByExtId(string extId, int customerId = 0);

        /// <summary>
        /// Get Users IdentityStatus
        /// </summary>
        /// <param name="extId"></param>
        /// <returns></returns>
        List<IdentityStatusDto> GetListUserIdentityStatusByExtId(string extId);

        /// <summary>
        /// Get User IdentityStatus
        /// </summary>
        /// <param name="ssn"></param>
        /// <param name="customerId">0 mean skip customer filtering</param>
        /// <returns></returns>
        IdentityStatusDto GetUserIdentityStatusBySsn(string ssn, int customerId = 0);
        Task<ConexusBaseDto> ArchiveUserByIdAsync(int userId, bool syncToIdp = true, EntityStatusReasonEnum? entityStatusReason = null, bool? isAutoArchived = null);
        Task<ConexusBaseDto> ArchiveUserAsync(
            UserDtoBase userDtoBase,
            int departmentId,
            bool syncToIdp = true,
            EntityStatusReasonEnum? entityStatusReason = null,
            IWorkContext workContext = null,
            bool? isAutoArchived = null);
        Task<ConexusBaseDto> UnarchiveAsync(int userId, bool syncToIdp = true, EntityStatusReasonEnum? entityStatusReason = null);
        /// <summary>
        /// Get User IdentityStatus
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        IdentityStatusDto GetUserIdentityStatusById(int userId);
        /// <summary>
        /// Get a default user belong to a customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        UserEntity GetDefaultUserByCustomer(int customerId);

        /// <summary>
        /// Get users.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ownerId"></param>
        /// <param name="customerIds"></param>
        /// <param name="userIds"></param>
        /// <param name="userGroupIds"></param>
        /// <param name="statusIds"></param>
        /// <param name="archetypeIds"></param>
        /// <param name="userTypeIds"></param>
        /// <param name="userTypeExtIds"></param>
        /// <param name="parentDepartmentIds"></param>
        /// <param name="extIds"></param>
        /// <param name="ssnList"></param>
        /// <param name="userNames"></param>
        /// <param name="lastUpdatedBefore"></param>
        /// <param name="lastUpdatedAfter"></param>
        /// <param name="ageRanges"></param>
        /// <param name="genders"></param>
        /// <param name="memberStatuses"></param>
        /// <param name="memberValidFromBefore"></param>
        /// <param name="memberValidFromAfter"></param>
        /// <param name="memberValidToBefore"></param>
        /// <param name="memberValidToAfter"></param>
        /// <param name="searchKey"></param>
        /// <param name="loginServiceClaims"></param>
        /// <param name="loginServiceClaimTypes"></param>
        /// <param name="loginServiceIds"></param>
        /// <param name="getDynamicProperties"></param>
        /// <param name="getLoginServiceClaims"></param>
        /// <param name="getRoles"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderBy"></param>
        /// <param name="filterOnParentHd"></param>
        /// <param name="includeUGMembers"></param>
        /// <param name="includeDepartment"></param>
        /// <param name="jsonDynamicData"></param>
        /// <param name="externallyMastered"></param>
        /// <param name="skipPaging"></param>
        /// <param name="createdAfter"></param>
        /// <param name="createdBefore"></param>
        /// <param name="expirationDateAfter"></param>
        /// <param name="expirationDateBefore"></param>
        /// <param name="orgUnittypeIds"></param>
        /// <param name="multiUserTypefilters"></param>
        /// <param name="filterOnSubDepartment"></param>
        /// <param name="multiUserGroupFilters"></param>
        /// <param name="multiUserTypeExtIdFilters"></param>
        /// <param name="currentWorkContext"></param>
        /// <param name="checkDepartmentPermission"></param>
        /// <param name="emails"></param>
        /// <param name="ignoreCheckReadUserAccess">NOTE: This flag for internal use only and should never be sent as the parameter in the controller action.</param>
        /// <param name="includeOwnUserGroups"></param>
        /// <param name="activeDateBefore"></param>
        /// <param name="activeDateAfter"></param>
        /// <param name="exceptUserIds"></param>
        /// <returns></returns>
        PaginatedList<T> GetUsers<T>(int ownerId = 0,
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
            bool? getDynamicProperties = null,
            bool? getLoginServiceClaims = null,
            bool? getRoles = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            bool? filterOnParentHd = true,
            bool includeUGMembers = false,
            bool includeDepartment = false,
            List<string> jsonDynamicData = null,
            bool? externallyMastered = null,
            bool skipPaging = false,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            List<int> orgUnittypeIds = null,
            List<List<int>> multiUserTypefilters = null,
            bool? filterOnSubDepartment = null,
            List<List<int>> multiUserGroupFilters = null,
            List<List<string>> multiUserTypeExtIdFilters = null,
            IWorkContext currentWorkContext = null,
            bool checkDepartmentPermission = false,
            List<string> emails = null,
            bool ignoreCheckReadUserAccess = false,
            bool includeOwnUserGroups = false,
            DateTime? activeDateBefore = null,
            DateTime? activeDateAfter = null,
            List<int> exceptUserIds = null) where T : ConexusBaseDto;

        PaginatedList<T> SearchActors<T>(int ownerId = 0,
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
            bool? getDynamicProperties = null,
            bool? getLoginServiceClaims = null,
            bool? getRoles = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            List<int> exceptUserIds = null) where T : ConexusBaseDto;
        List<ConexusBaseDto> SearchUsers(string searchKey, int departmentId, int hdId = 0, bool deepSearch = false, int maxTake = 0);
        List<ConexusBaseDto> GetUsersByUserGroups(List<int> userGroupIds, List<int> usertypeIds, List<int> gender = null, List<string> ages = null, string countries = "");
        ConexusBaseDto InsertUser(UserDtoBase userDto);
        List<LevelDto> GetUserLevel(int userId, ArchetypeEnum userArcheType, ArchetypeEnum useTypeArcheType);

        void MoveLearnersToSchool(int schoolId,
            List<int> learnerIds,
            int graduateUsertypeId,
            int newStudentUserTypeId,
            int receivingDepartmentPropertyId,
            int currentPeriodId,
            int toPeriodId,
            int fromPeriodId);
        void MoveLearnersToClass(int classId, List<int> learnerIds, int newStudentUserTypeId);

        List<UserEntity> GetListUsersByDepartmentIds(IEnumerable<int> departmentIds, bool includeLinkedUsers = false);
        bool CheckUsername(int ownerId, int userId, string userName);
        UserEntity GetUserForUpdateInsert(int userId);
        UserEntity UpdateUser(UserEntity user, bool changePassword = false, bool generateRandomPassword = true, string updatedPassword = "");
        UserEntity InsertUser(UserEntity user);
        List<UserEntity> GetUsersByUsernameForUpdate(int ownerId, string username, params EntityStatusEnum[] filters);
        UserEntity GetUserIncludeDepartmentIncludeUserTypesIncludeUserGroups(int userId, bool putToCache = true);
        LogonResponseDto NativeLogin(string userName, string password);
        List<IdentityStatusDto> UpdateUserLastSyncDate(List<IdentityStatusDto> users);
        Dictionary<string, List<HierachyDepartmentIdentityDto>> GetUserHierachyDepartmentIdentitiesByExtIds(List<string> extIds);

        Dictionary<string, List<MemberDto>> GetUsersMemberships(List<string> userExtIds, ArchetypeEnum userArcheType);
        List<IdentityStatusDto> GetUserIdentitiesByObjectMapping(List<int> userIds);

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
            bool? filterOnParentHd = true,
            List<string> jsonDynamicData = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            bool? externallyMastered = null,
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
            bool? filterOnParentHd = true,
            List<string> jsonDynamicData = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            bool? externallyMastered = null,
            List<int> exceptUserIds = null);
        dynamic BuildCommunicationCommandRecipient(UserDtoBase executorUser, UserDtoBase objectiveUser, SendEmailToDto sendEmailToDto);

        void ManuallySendWelcomeEmail(IWorkContext workContext,
            List<int> userIds = null,
            List<string> userExtIds = null,
            List<int> parentDepartmentIds = null,
            List<string> emails = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            int pageSize = 0,
            int pageIndex = 0,
            string orderBy = "",
            List<bool> externallyMasteredValues = null,
            List<EntityStatusEnum> userEntityStatuses = null);
        Task<PaginatedList<T>> GetUsersAsync<T>(int ownerId = 0,
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
            bool? getDynamicProperties = null,
            bool? getLoginServiceClaims = null,
            bool? getRoles = null,
            int pageIndex = 0,
            int pageSize = 0,
            string orderBy = "",
            bool? filterOnParentHd = true,
            bool includeUGMembers = false,
            bool includeDepartment = false,
            List<string> jsonDynamicData = null,
            bool? externallyMastered = null,
            bool skipPaging = false,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            List<int> orgUnittypeIds = null,
            List<List<int>> multiUserTypefilters = null,
            bool? filterOnSubDepartment = null,
            List<string> departmentExtIds = null,
            List<List<int>> multiUserGroupFilters = null,
            List<List<string>> multiUserTypeExtIdFilters = null,
            IWorkContext currentWorkContext = null,
            List<string> emails = null,
            bool ignoreCheckReadUserAccess = false,
            bool includeOwnUserGroups = false,
            DateTime? activeDateBefore = null,
            DateTime? activeDateAfter = null,
            List<int> exceptUserIds = null,
            bool isCrossOrganizationalUnit = false,
            List<string> systemRolePermissions = null,
            string token = null) where T : ConexusBaseDto;
        void SchedulySendWelcomeEmail(IWorkContext workContext, DateTime? entityActiveDateBefore = null,
            DateTime? entityActiveDateAfter = null);

        Task<CountUserResultDto> CountUserGroupByAsync(int ownerId,
            List<int> customerIds = null,
            List<int> userIds = null,
            List<int> userGroupIds = null,
            List<EntityStatusEnum> statusIds = null,
            List<ArchetypeEnum> archetypeIds = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,

            List<int> departmentIds = null,
            List<string> extIds = null,
            List<string> jsonDynamicData = null,
            List<int> exceptUserIds = null,
            List<List<int>> multiUserTypeFilters = null,
            List<List<string>> multiUserTypeExIdFilters = null,

            List<List<int>> multiUserGroupFilters = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? createdBefore = null,
            DateTime? createdAfter = null,
            UserGroupByField groupByField = UserGroupByField.None);
    }
}
