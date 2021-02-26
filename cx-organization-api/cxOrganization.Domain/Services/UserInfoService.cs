using cxOrganization.Domain.Dtos.Departments;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Repositories.QueryBuilders;
using cxOrganization.Domain.Security.AccessServices;
using cxOrganization.Domain.Security.User;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Exceptions;
using cxPlatform.Core.Extentions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using cxOrganization.Domain.Mappings;
using cxOrganization.Domain.HttpClients;

namespace cxOrganization.Domain.Services
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserInfoService> _logger;
        private readonly IWorkContext _workContext;
        private readonly AppSettings _appSettings;
        private IOptions<AppSettings> _appSettingOption;
        private readonly IUserCryptoService _userCryptoService;
        private readonly IUserAccessService _userAccessService;
        private readonly IUserTypeMappingService _userTypeMappingService;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly OrganizationDbContext _organizationDbContext;
        private readonly IInternalHttpClientRequestService _internalHttpClientRequestService;

        public UserInfoService(IUserRepository userRepository,
            ILogger<UserInfoService> logger,
            IWorkContext workContext,
            IUserCryptoService userCryptoService,
            IUserAccessService userAccessService,
            IOptions<AppSettings> appSettingOption,
            IUserTypeRepository userTypeRepository,
            IUserTypeMappingService userTypeMappingService,
        IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            OrganizationDbContext organizationDbContext,
            IInternalHttpClientRequestService internalHttpClientRequestService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _workContext = workContext;
            _appSettings = appSettingOption.Value;
            _appSettingOption = appSettingOption;
            _userAccessService = userAccessService;
            _userCryptoService = userCryptoService;
            _userTypeRepository = userTypeRepository;
            _userTypeMappingService = userTypeMappingService;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _organizationDbContext = organizationDbContext;
            _internalHttpClientRequestService = internalHttpClientRequestService;
        }
        public async Task<(int, UserInfoDto)> GetUserInfoAsync(string extId,
            bool includeBasicInfo = false,
            bool includeTagIds = true,
            bool includeUserTagGroups = false)
        {
            var extIds = new List<string> { extId };

            var readUserAccessChecking = await _userAccessService.CheckReadUserAccessAsync(workContext: _workContext, ownerId: _workContext.CurrentOwnerId,
                customerIds: new List<int> { _workContext.CurrentCustomerId },
                userExtIds: extIds,
                loginServiceClaims: null,
                userIds: null,
                userGroupIds: null,
                parentDepartmentIds: null,
                multiUserGroupFilters: null,
                userTypeIdsFilter: null,
                userTypeExtIdsFilter: null,
                multipleUserTypeIdsFilter: null,
                multipleUserTypeExtIdsFilter: null);

            if (readUserAccessChecking.AccessStatus != AccessStatus.AccessGranted)
            {
                throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, string.Format("ExtId : {0}", extId));
            }

            var userIds = readUserAccessChecking.UserIds;
            var parentDepartmentIds = readUserAccessChecking.ParentDepartmentIds;
            var multiUserGroupFilters = readUserAccessChecking.MultiUserGroupFilters;
            var multiUserTypeFilters = readUserAccessChecking.MultiUserTypeFilters;

            var result = await _userRepository.GetUsersAsync(extIds: extIds,
                includeUserTypes: IncludeUserTypeOption.UserType,
                userIds: userIds,
                parentDepartmentIds: parentDepartmentIds,
                multiUserGroupFilters: multiUserGroupFilters,
                multiUserTypeFilters: multiUserTypeFilters,
                filterOnParentHd: false);

            if (result.Items == null || !result.Items.Any())
                throw new CXValidationException(cxExceptionCodes.ERROR_NOT_FOUND, string.Format("ExtId : {0}", extId));
            var user = result.Items.FirstOrDefault();
            var dto = new UserInfoDto();
            if (includeBasicInfo)
            {
                MapBasicInfo(user, dto);
            }
            if (includeTagIds)
            {
                MapTagIds(user, dto, includeUserTagGroups);
            }
            return (user.UserId, dto);
        }

        /// <summary>
        /// Gets public user info which doesn't need to check permission on data.
        /// </summary>
        /// <param name="userCxIds">The list of user CxIds.</param>
        /// <returns></returns>
        public async Task<List<PublicUserInfo>> GetPublicUserInfoAsync(List<string> userCxIds)
        {
            if (userCxIds.IsNullOrEmpty())
            {
                return new List<PublicUserInfo>();
            }

            var paginatedUsers = await _userRepository.GetUsersAsync(
                ownerId: _workContext.CurrentOwnerId,
                customerIds: new List<int> { _workContext.CurrentCustomerId },
                statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All },
                loginServiceClaims: userCxIds,
                includeLoginServiceUsers: true,
                includeDepartment: IncludeDepartmentOption.Department,
                includeUserTypes: IncludeUserTypeOption.UtUs);

            var users = paginatedUsers.Items;
            var typeOfOrganizationDictionary = GetTypeOfOrganizationDictionary(users.Select(u => u.Department).DistinctBy(dept => dept.DepartmentId).ToList());
            var serviceSchemes = _userTypeRepository.GetAllUserTypesInCache()
                .Where(ut => ut.ArchetypeId == (int)ArchetypeEnum.PersonnelGroup).ToList();

            var publicUserInfos = new List<PublicUserInfo>();

            foreach (var user in users)
            {
                var developmentUserType = _userTypeRepository.GetUserTypes(
                        ownerId: _workContext.CurrentOwnerId,
                        userIds: new List<int>() { user.UserId },
                        archetypeIds: new List<ArchetypeEnum>() { ArchetypeEnum.DevelopmentalRole },
                        userTypeIds: null,
                        extIds: null,
                        includeLocalizedData: true,
                        parentIds: null);
                var developmentUserTypeDtos = _userTypeMappingService.ToUserTypeDtos(developmentUserType);


                var jsonDynamic = JObject.Parse(user.DynamicAttributes);
                var userDepartment = user.Department;

                var publicUserInfo = new PublicUserInfo
                {
                    FullName = string.Format("{0} {1}", user.FirstName, user.LastName).Trim(),
                    UserCxId = user.LoginServiceUsers.FirstOrDefault()?.PrimaryClaimValue,
                    EmailAddress = user.Email,
                    DepartmentId = user.DepartmentId,
                    DepartmentName = userDepartment.Name,
                    OrganizationAddress = userDepartment.Adress,
                    TypeOfOrganization = typeOfOrganizationDictionary[userDepartment.DepartmentId],
                    LastLoginDate = jsonDynamic["lastLoginDate"] is object ? jsonDynamic["lastLoginDate"].ToObject<DateTime>() : (DateTime?)null,
                    ServiceScheme = GetUserServiceScheme(serviceSchemes, user.UT_Us),
                    DevelopmentRoles = developmentUserTypeDtos.Any()
                        ? developmentUserTypeDtos.Select(developmentUserTypeDto => developmentUserTypeDto.Identity.ExtId).ToList()
                        : null
                };
                BuildUserInfoFromJsonDynamicAttributes(user, publicUserInfo);
                publicUserInfos.Add(publicUserInfo);
            }

            return publicUserInfos;
        }

        private void BuildUserInfoFromJsonDynamicAttributes(UserEntity user, PublicUserInfo publicUserInfo)
        {
            var userIsExternallyMastered = user.Locked == 1;

            if (!string.IsNullOrEmpty(user.DynamicAttributes))
            {
                var jsonDynamicAttributes = new Dictionary<string, dynamic>(StringComparer.CurrentCultureIgnoreCase);
                //PopulateObject to map data to defined ignore case dictionary
                JsonConvert.PopulateObject(user.DynamicAttributes, jsonDynamicAttributes);

                jsonDynamicAttributes.TryGetValue(UserJsonDynamicAttributeName.AvatarUrl, out var avatarUrl);
                publicUserInfo.AvatarUrl = Convert.ToString(avatarUrl);

                var designationKey = userIsExternallyMastered ? UserJsonDynamicAttributeName.JobTitle : UserJsonDynamicAttributeName.Designation;
                jsonDynamicAttributes.TryGetValue(designationKey, out var designation);
                publicUserInfo.Designation = Convert.ToString(designation);

                publicUserInfo.Portfolios = GetStringValues(jsonDynamicAttributes, UserJsonDynamicAttributeName.Portfolios);
                publicUserInfo.RoleSpecificProficiencies = GetStringValues(jsonDynamicAttributes, UserJsonDynamicAttributeName.RoleSpecificProficiencies);
                publicUserInfo.JobFamilies = GetStringValues(jsonDynamicAttributes, UserJsonDynamicAttributeName.JobFamily);
                publicUserInfo.TeachingSubjects = GetStringValues(jsonDynamicAttributes, UserJsonDynamicAttributeName.TeachingSubjects);
                publicUserInfo.TeachingLevels = GetStringValues(jsonDynamicAttributes, UserJsonDynamicAttributeName.TeachingLevels);
                publicUserInfo.TeachingCourseOfStudy = GetStringValues(jsonDynamicAttributes, UserJsonDynamicAttributeName.TeachingCourseOfStudy);
                publicUserInfo.CoCurricularActivities = GetStringValues(jsonDynamicAttributes, UserJsonDynamicAttributeName.CoCurricularActivities);
                publicUserInfo.AreasOfProfessionalInterest = GetStringValues(jsonDynamicAttributes, UserJsonDynamicAttributeName.AreasOfProfessionalInterest);
                publicUserInfo.NotificationPreferences = GetStringValues(jsonDynamicAttributes, UserJsonDynamicAttributeName.NotificationPreferences);
            }
        }

        private static string GetUserServiceScheme(List<UserTypeEntity> serviceSchemes, ICollection<UTUEntity> utus)
        {
            return (from utu in utus
                    join userType in serviceSchemes on utu.UserTypeId equals userType.UserTypeId
                    select userType.ExtId).FirstOrDefault();
        }

        private Dictionary<int, string> GetTypeOfOrganizationDictionary(List<DepartmentEntity> departmentEntities)
        {
            var typeOfOrganizationDictionary = new Dictionary<int, string>();

            foreach (var department in departmentEntities)
            {
                if (string.IsNullOrEmpty(department.DynamicAttributes))
                {
                    typeOfOrganizationDictionary.Add(department.DepartmentId, null);
                    continue;
                }

                var departmentJsonDynamicAttributes = new Dictionary<string, dynamic>(StringComparer.CurrentCultureIgnoreCase);
                //PopulateObject to map data to defined ignore case dictionary
                JsonConvert.PopulateObject(department.DynamicAttributes, departmentJsonDynamicAttributes);
                var typeOfOrganization = GetJsonDynamicAttribute<string>(departmentJsonDynamicAttributes, DepartmentJsonDynamicAttributeName.TypeOfOrganizationUnit);
                typeOfOrganizationDictionary.Add(department.DepartmentId, typeOfOrganization);
            }
            return typeOfOrganizationDictionary;
        }


        public async Task<PaginatedList<UserBasicInfo>> GetUserWithBasicInfos(List<int> userIds = null,
            List<string> extIds = null, 
            List<string> emails = null, 
            List<EntityStatusEnum> entityStatuses = null,
            string searchKey = null,
            List<int> departmentIds = null,
            bool? externallyMastered = null,
            List<int> userTypeIds = null,
            List<string> userTypeExtIds = null,
            List<List<int>> multiUserTypeFilters = null,
            List<List<string>> multiUserTypeExtIdFilters = null,
            int pageSize = 0,
            int pageIndex = 0,
            string orderBy = "", 
            bool getFullIdentity = false,
            bool getEntityStatus = false,
            List<int> userGroupIds = null,
            List<int> exceptUserIds = null, 
            List<string> systemRolePermissions = null,
            string token = null)
        {
            var customerIds = new List<int>(_workContext.CurrentCustomerId);
            var userAccessChecking = await _userAccessService.CheckReadUserAccessAsync(workContext: _workContext,
                ownerId: _workContext.CurrentOwnerId, customerIds: customerIds,
                userExtIds: extIds,
                loginServiceClaims: null,
                userIds: userIds,
                userGroupIds: userGroupIds,
                parentDepartmentIds: departmentIds,
                multiUserGroupFilters: null,
                userTypeIdsFilter: userTypeIds,
                userTypeExtIdsFilter: userTypeExtIds,
                multipleUserTypeIdsFilter: multiUserTypeFilters,
                multipleUserTypeExtIdsFilter: multiUserTypeExtIdFilters,
                accessPolicy: "basicInfoPolicy");

            if (userAccessChecking.AccessStatus != AccessStatus.AccessGranted)
            {
                return new PaginatedList<UserBasicInfo>();
            }

            userIds = userAccessChecking.UserIds;
            departmentIds = userAccessChecking.ParentDepartmentIds;
            multiUserTypeFilters = userAccessChecking.MultiUserTypeFilters;
            entityStatuses = entityStatuses ?? new List<EntityStatusEnum>();
            userGroupIds = userAccessChecking.UserGroupIds;
            var multiplUserGroupIds = userAccessChecking.MultiUserGroupFilters;
            if (entityStatuses.Count == 0)
            {
                entityStatuses.Add(EntityStatusEnum.Active);
                entityStatuses.Add(EntityStatusEnum.New);
            }

            if (systemRolePermissions is object)
            {
                var baseUrl = _appSettingOption.Value.PortalAPI + "/SystemRoles/FindByPermissionKeys";
                var systemRoleIdsBasedOnPermissions = await _internalHttpClientRequestService.GetAsync<List<int>>(token,
                                                                                       baseUrl,
                                                                                       ("PermissionKeys", systemRolePermissions),
                                                                                       ("LogicalOperator", new List<string>() { "AND" }));
                if (multiUserTypeFilters is null)
                {
                    multiUserTypeFilters = new List<List<int>>();
                }

                if (!multiUserTypeFilters.Any())
                {
                    multiUserTypeFilters.Add(systemRoleIdsBasedOnPermissions);
                }
                else
                {
                    multiUserTypeFilters[0] = multiUserTypeFilters[0].Intersect(systemRoleIdsBasedOnPermissions).ToList();

                    if (!multiUserTypeFilters[0].Any())
                    {
                        multiUserTypeFilters[0].Add(-99);
                    }
                }

            }

            var query = UserQueryBuilder.InitQueryBuilder(_appSettingOption, _userCryptoService,
                    _userRepository.GetQueryAsNoTracking(entityStatuses.ToArray()))
                .FilterByUserIds(userIds, exceptUserIds)
                .FilterByUserExtIds(extIds)
                .FilterByEmails(emails)
                .FilterByDepartmentIds(departmentIds, false, false)
                .FilterBySearchKey(searchKey, _appSettings.EnableSearchingSSN)
                .FilterByMultiUserTypeFilters(multiUserTypeFilters)

                .FilterByUserGroupIds(userGroupIds: userGroupIds, memberStatuses: null,
                    memberValidFromBefore: null, memberValidFromAfter: null, memberValidToBefore: null,
                    memberValidToAfter: null)
                .FilterByMultiUserGroupFilters(multiUserGroupFilters: multiplUserGroupIds, memberStatuses: null,
                    memberValidFromBefore: null, memberValidFromAfter: null, memberValidToBefore: null,
                    memberValidToAfter: null)
                .Build();

            if (externallyMastered.HasValue)
            {
                var locked = externallyMastered.Value ? 1 : 0;
                query = query.Where(x => x.Locked == locked);
            }

            query = query.Include(q => q.Department);

            //Query must be ordered before apply paging
            query = !string.IsNullOrEmpty(searchKey)
                ? query.OrderBy(x => x.FirstName)
                : LinqExtension.ApplyOrderBy(query, p => p.UserId, orderBy);

            return await ExecuteGetUserWithBasicInfo(query, pageSize, pageIndex, getFullIdentity, getEntityStatus);
        }

        public async Task<PaginatedList<UserHierarchyInfo>> GetUserHierarchyInfos(List<int> userIds = null,
            List<string> extIds = null, List<string> emails = null, List<EntityStatusEnum> entityStatuses = null,
            List<int> departmentIds = null,
            int pageSize = 0, int pageIndex = 0, string orderBy = "")
        {
            var customerIds = new List<int>(_workContext.CurrentCustomerId);
            var userAccessChecking = await _userAccessService.CheckReadUserAccessAsync(workContext: _workContext,
                ownerId: _workContext.CurrentOwnerId, customerIds: customerIds,
                userExtIds: extIds,
                loginServiceClaims: null,
                userIds: userIds,
                userGroupIds: null,
                parentDepartmentIds: departmentIds,
                multiUserGroupFilters: null,
                userTypeIdsFilter: null,
                userTypeExtIdsFilter: null,
                multipleUserTypeIdsFilter: null,
                multipleUserTypeExtIdsFilter: null);

            if (userAccessChecking.AccessStatus != AccessStatus.AccessGranted)
            {
                return new PaginatedList<UserHierarchyInfo>();
            }

            userIds = userAccessChecking.UserIds;
            departmentIds = userAccessChecking.ParentDepartmentIds;
            var multiUserTypeFilters = userAccessChecking.MultiUserTypeFilters;
            entityStatuses = entityStatuses ?? new List<EntityStatusEnum>();

            if (entityStatuses.Count == 0)
            {
                entityStatuses.Add(EntityStatusEnum.Active);
                entityStatuses.Add(EntityStatusEnum.New);
            }

            var query = UserQueryBuilder.InitQueryBuilder(_appSettingOption, _userCryptoService,
                    _userRepository.GetQueryAsNoTracking(entityStatuses.ToArray()))
                .FilterByUserIds(userIds)
                .FilterByUserExtIds(extIds)
                .FilterByEmails(emails)
                .FilterByDepartmentIds(departmentIds, false, false)
                .FilterByMultiUserTypeFilters(multiUserTypeFilters)
                .Build();

            query = LinqExtension.ApplyOrderBy(query, p => p.UserId, orderBy);

            var selectQuery = query.Select(q => new UserHierarchyInfo { UserId = q.UserId, UserCxId = q.ExtId, DepartmentId = q.DepartmentId });

            var paginatedUserHierarchyInfos = await selectQuery.ToPagingAsync(pageIndex, pageSize);
            if (paginatedUserHierarchyInfos.Items.Count > 0)
            {
                var userDepartmentIds = paginatedUserHierarchyInfos.Items
                    .Select(u => u.DepartmentId)
                    .Distinct()
                    .ToList();

                var hierarchyInfos = _hierarchyDepartmentRepository.GetHierarchyInfos(_workContext.CurrentHdId, userDepartmentIds, true);

                if (hierarchyInfos.Count > 0)
                {
                    foreach (var userHierarchyInfo in paginatedUserHierarchyInfos.Items)
                    {

                        userHierarchyInfo.HierarchyInfo = hierarchyInfos.FirstOrDefault(h => h.DepartmentId == userHierarchyInfo.DepartmentId);
                    }
                }
            }

            return new PaginatedList<UserHierarchyInfo>(paginatedUserHierarchyInfos.Items, pageIndex, pageSize, paginatedUserHierarchyInfos.HasMoreData)
            {
                TotalItems = paginatedUserHierarchyInfos.TotalItems
            };
        }

        private static async Task<PaginatedList<UserBasicInfo>> ExecuteGetUserWithBasicInfo(
            IQueryable<UserEntity> query, int pageSize, int pageIndex, bool getFullIdentity, bool getEntityStatus)
        {
            var avatarPath = $"$.{UserJsonDynamicAttributeName.AvatarUrl}";

            IQueryable<UserBasicInfo> selectQuery;
            if (getFullIdentity && getEntityStatus)
            {
                selectQuery = BuildUserBasicInfoQuery(query, avatarPath);
            }
            else if (getFullIdentity)
            {
                selectQuery = BuildUserBasicInfoWithOutEntityStatusQuery(query, avatarPath);
            }
            else if (getEntityStatus)
            {
                selectQuery = BuildUserBasicInfoWithoutFullIdentityQuery(query, avatarPath);
            }
            else
            {
                selectQuery = BuildUserBasicInfoWithoutFullIdentityAndEntityStatusQuery(query, avatarPath);
            }

            var paginatedUserEntityInfos = await selectQuery.ToPagingAsync(pageIndex, pageSize);

            return new PaginatedList<UserBasicInfo>(paginatedUserEntityInfos.Items, pageIndex, pageSize,
                paginatedUserEntityInfos.HasMoreData)
            { TotalItems = paginatedUserEntityInfos.TotalItems };
        }

        private static IQueryable<UserBasicInfo> BuildUserBasicInfoQuery(IQueryable<UserEntity> query, string avatarPath)
        {
            return query.Select(q =>
                new UserBasicInfo
                {
                    Identity = new IdentityDto()
                    {
                        Id = q.UserId,
                        CustomerId = q.CustomerId ?? 0,
                        OwnerId = q.OwnerId,
                        ExtId = q.ExtId,
                        Archetype = (ArchetypeEnum)(q.ArchetypeId ?? 0)
                    },
                    DepartmentId = q.DepartmentId,
                    DepartmentName = q.Department.Name,
                    EmailAddress = q.Email,
                    FirstName = q.FirstName,
                    LastName = q.LastName,
                    AvatarUrl = EfJsonExtensions.JsonValue(q.DynamicAttributes, avatarPath),
                    UserCxId = q.ExtId,
                    EntityStatus = new EntityStatusDto()
                    {
                        ActiveDate = q.EntityActiveDate,
                        ExpirationDate = q.EntityExpirationDate,
                        LastUpdated = q.LastUpdated,
                        LastUpdatedBy = q.LastUpdatedBy ?? 0,
                        ExternallyMastered = q.Locked == 1,
                        StatusId = (EntityStatusEnum)(q.EntityStatusId ?? 0),
                        StatusReasonId = (EntityStatusReasonEnum)(q.EntityStatusReasonId ?? 0),
                        LastExternallySynchronized = q.LastSynchronized,
                        EntityVersion = q.EntityVersion
                    }
                });

        }
        private static IQueryable<UserBasicInfo> BuildUserBasicInfoWithoutFullIdentityQuery(IQueryable<UserEntity> query, string avatarPath)
        {
            return query.Select(q =>
                new UserBasicInfo
                {

                    DepartmentId = q.DepartmentId,
                    DepartmentName = q.Department.Name,
                    EmailAddress = q.Email,
                    FirstName = q.FirstName,
                    LastName = q.LastName,
                    AvatarUrl = EfJsonExtensions.JsonValue(q.DynamicAttributes, avatarPath),
                    UserCxId = q.ExtId,
                    EntityStatus = new EntityStatusDto()
                    {
                        ActiveDate = q.EntityActiveDate,
                        ExpirationDate = q.EntityExpirationDate,
                        LastUpdated = q.LastUpdated,
                        LastUpdatedBy = q.LastUpdatedBy ?? 0,
                        ExternallyMastered = q.Locked == 1,
                        StatusId = (EntityStatusEnum)(q.EntityStatusId ?? 0),
                        StatusReasonId = (EntityStatusReasonEnum)(q.EntityStatusReasonId ?? 0),
                        LastExternallySynchronized = q.LastSynchronized,
                        EntityVersion = q.EntityVersion
                    }
                });

        }
        private static IQueryable<UserBasicInfo> BuildUserBasicInfoWithoutFullIdentityAndEntityStatusQuery(IQueryable<UserEntity> query, string avatarPath)
        {
            return query.Select(q =>
                new UserBasicInfo
                {

                    DepartmentId = q.DepartmentId,
                    DepartmentName = q.Department.Name,
                    EmailAddress = q.Email,
                    FirstName = q.FirstName,
                    LastName = q.LastName,
                    AvatarUrl = EfJsonExtensions.JsonValue(q.DynamicAttributes, avatarPath),
                    UserCxId = q.ExtId

                });

        }
        private static IQueryable<UserBasicInfo> BuildUserBasicInfoWithOutEntityStatusQuery(IQueryable<UserEntity> query, string avatarPath)
        {
            return query.Select(q =>
                new UserBasicInfo
                {
                    Identity = new IdentityDto()
                    {
                        Id = q.UserId,
                        CustomerId = q.CustomerId ?? 0,
                        OwnerId = q.OwnerId,
                        ExtId = q.ExtId,
                        Archetype = (ArchetypeEnum)(q.ArchetypeId ?? 0)
                    },
                    DepartmentId = q.DepartmentId,
                    DepartmentName = q.Department.Name,
                    EmailAddress = q.Email,
                    FirstName = q.FirstName,
                    LastName = q.LastName,
                    AvatarUrl = EfJsonExtensions.JsonValue(q.DynamicAttributes, avatarPath),
                    UserCxId = q.ExtId
                });

        }
        private List<string> GetStringValues(Dictionary<string, dynamic> jsonDynamicAttributes, string searchKey)
        {
            jsonDynamicAttributes.TryGetValue(searchKey, out var value);

            var jArray = value as JArray;
            if (jArray == null) return null;

            return jArray.Values<string>().ToList();
        }

        private T GetJsonDynamicAttribute<T>(Dictionary<string, dynamic> jsonDynamicAttributes, string attributeName)
        {
            if (!jsonDynamicAttributes.IsNullOrEmpty() &&
                jsonDynamicAttributes.TryGetValue(attributeName, out dynamic attribute))
            {
                return (T)attribute;
            }

            return default(T);
        }

        private void MapTagIds(UserEntity user, UserInfoDto dto, bool includeUserTagGroups = false)
        {
            if (dto.TagIds == null)
                dto.TagIds = new List<string>();
            Tags tags = null;
            if (string.IsNullOrEmpty(user.DynamicAttributes))
                tags = new Tags();
            else
                tags = JsonConvert.DeserializeObject<Tags>(user.DynamicAttributes);
            if (tags == null)
                return;

            //if (!string.IsNullOrEmpty(tags.Designation))
            //    dto.TagIds.Add(tags.Designation);

            if (tags.TeachingCourseOfStudy != null && tags.TeachingCourseOfStudy.Any())
                dto.TagIds.AddRange(tags.TeachingCourseOfStudy.Where(t => !string.IsNullOrEmpty(t)));

            if (tags.CocurricularActivities != null && tags.CocurricularActivities.Any())
                dto.TagIds.AddRange(tags.CocurricularActivities.Where(t => !string.IsNullOrEmpty(t)));

            if (tags.TeachingLevels != null && tags.TeachingLevels.Any())
                dto.TagIds.AddRange(tags.TeachingLevels.Where(t => !string.IsNullOrEmpty(t)));

            if (tags.TeachingSubjects != null && tags.TeachingSubjects.Any())
                dto.TagIds.AddRange(tags.TeachingSubjects.Where(t => !string.IsNullOrEmpty(t)));

            if (tags.ProfessionalInterests != null && tags.ProfessionalInterests.Any())
                dto.TagIds.AddRange(tags.ProfessionalInterests.Where(t => !string.IsNullOrEmpty(t)));

            var usertypes = user.UT_Us?.Where(t => t.UserType != null
            && (
                t.UserType.ArchetypeId == (int)ArchetypeEnum.PersonnelGroup
                || t.UserType.ArchetypeId == (int)ArchetypeEnum.LearningFramework
                || t.UserType.ArchetypeId == (int)ArchetypeEnum.DevelopmentalRole
                || t.UserType.ArchetypeId == (int)ArchetypeEnum.CareerPath)
               ).Select(t => t.UserType).ToList();

            if (usertypes.Any())
            {
                foreach (var usertype in usertypes)
                {
                    if (string.IsNullOrEmpty(usertype.ExtId))
                        continue;

                    dto.TagIds.Add(usertype.ExtId);

                    switch (usertype.ArchetypeId)
                    {
                        case (int)ArchetypeEnum.PersonnelGroup:
                            tags.ServiceSchemes = AddValue(tags.ServiceSchemes, usertype.ExtId);
                            break;
                        case (int)ArchetypeEnum.LearningFramework:
                            tags.LearningFrameworks = AddValue(tags.LearningFrameworks, usertype.ExtId);
                            break;
                        case (int)ArchetypeEnum.DevelopmentalRole:
                            tags.DevelopmentalRoles = AddValue(tags.DevelopmentalRoles, usertype.ExtId);
                            break;
                        case (int)ArchetypeEnum.CareerPath:
                            tags.Tracks = AddValue(tags.Tracks, usertype.ExtId);
                            break;
                    }
                }
            }
            if (includeUserTagGroups)
            {
                dto.TagGroups = tags;

                // Job Families is not included in tags, so we have to map it manually
                dto.TagGroups.JobFamilies = tags.JobFamilies;
            }    

        }

        private List<string> AddValue(List<string> list, string extId)
        {
            if (list == null)
            {
                list = new List<string>();
            }
            if (!string.IsNullOrEmpty(extId))
                list.Add(extId);
            return list;
        }

        private void MapBasicInfo(UserEntity user, UserInfoDto dto)
        {
            dto.BasicInfo = new UserInfo
            {
                Created = user.Created,
                DateOfBirth = ShouldHideDateOfBirth() ? null : user.DateOfBirth,
                DepartmentId = user.DepartmentId,
                EmailAddress = user.Email,
                Gender = user.Gender,
                LastName = user.LastName,
                FirstName = user.FirstName,
                Tag = user.Tag,
                EntityStatus = new EntityStatusDto
                {
                    EntityVersion = user.EntityVersion,
                    LastUpdated = user.LastUpdated,
                    LastUpdatedBy = user.LastUpdatedBy ?? 0,
                    StatusId = (EntityStatusEnum)user.EntityStatusId,
                    StatusReasonId = user.EntityStatusReasonId.HasValue ? (EntityStatusReasonEnum)user.EntityStatusReasonId : EntityStatusReasonEnum.Unknown,
                    LastExternallySynchronized = user.LastSynchronized,
                    ExternallyMastered = user.Locked == 1,
                    Deleted = user.Deleted.HasValue,
                    ExpirationDate = user.EntityExpirationDate,
                    ActiveDate = user.EntityActiveDate
                },
                Identity = new IdentityDto
                {
                    Archetype = (ArchetypeEnum)user.ArchetypeId,
                    ExtId = user.ExtId,
                    Id = user.UserId,
                    OwnerId = user.OwnerId,
                    CustomerId = user.CustomerId ?? 0
                }
            };
            dto.BasicInfo.SSN = ShouldHideSsn() ? null : _userCryptoService.DecryptSSN(user.SSN);
        }
        private bool ShouldHideSsn()
        {
            //We should only hide SSN when api is authenticate by user token to keep backward compatibility for system to system integration 
            return _appSettings.HideSSN && !string.IsNullOrEmpty(_workContext.Sub);
        }

        private bool ShouldHideDateOfBirth()
        {
            //We should only hide date of birth when api is authenticate by user token to keep backward compatibility for system to system integration 
            return _appSettings.HideDateOfBirth && !string.IsNullOrEmpty(_workContext.Sub);
        }

        public async Task<List<UserCountByUserTypeDto>> GetUserCountByUserTypes(
            List<ArchetypeEnum> userTypeArchetypes,
            List<int> userGroupIds,
            List<int> userIds)
        {
            if (userGroupIds.IsNullOrEmpty() && userIds.IsNullOrEmpty())
            {
                return new List<UserCountByUserTypeDto>();
            }

            try
            {
                var sqlParameters = new List<SqlParameter>();

                sqlParameters.AddSingleValueParameter("@OwnerId", _workContext.CurrentOwnerId);
                sqlParameters.AddSingleValueParameter("@CustomerId", _workContext.CurrentCustomerId);
                sqlParameters.AddSingleValueParameter("@LanguageId", _workContext.CurrentLanguageId);
                sqlParameters.AddMultiValuesParameter("@UserGroupIds", userGroupIds);
                sqlParameters.AddMultiValuesParameter("@UserIds", userIds);
                if (!userTypeArchetypes.IsNullOrEmpty())
                {
                    var userTypeArchetypeIds = userTypeArchetypes.Select(userTypeArchetype => (int)userTypeArchetype).ToList();
                    sqlParameters.AddMultiValuesParameter("@UserTypeArchetypeIds", userTypeArchetypeIds);
                }

                return await _organizationDbContext.ExecStoreStoredProcedureAsync<UserCountByUserTypeDto>(
                    "[org].[prc_User_CountByUserType]",
                    sqlParameters.ToArray());
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Could not get user count by user types.");
                throw;
            }
        }

    }
}
