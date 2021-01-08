using cxOrganization.Business.Extensions;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Enums;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Security.AccessServices;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.Business.PDPlanner.EmployeeList
{
    public class EmployeeListService : IEmployeeListService
    {

        //This define the mapping IdpEmployeeListSortField to store procedure sort field
        private static readonly Dictionary<IdpEmployeeListSortField, string[]> SortFieldEnumMappings =
            new Dictionary<IdpEmployeeListSortField, string[]>
            {
                {IdpEmployeeListSortField.FirstName, new[] {"FirstName"}},
                {IdpEmployeeListSortField.LastName, new[] {"LastName"}},
                {IdpEmployeeListSortField.FullName, new[] {"TRIM(FirstName +' '+LastName)"}},
                {IdpEmployeeListSortField.Department, new[] {"DepartmentName"}},
                {
                    IdpEmployeeListSortField.EntityStatus,
                    new[] {"EntityStatusId"} //TODO: should order with entity display name?
                },
                {IdpEmployeeListSortField.LearningNeedDueDate, new[] {"NeedResultDueDate"}},
                {IdpEmployeeListSortField.LearningNeedStatusType, new[] {"NeedStatusTypeNo", "NeedStatusTypeName"}},
                {IdpEmployeeListSortField.LearningPlanDueDate, new[] {"PlanResultDueDate"}},
                {IdpEmployeeListSortField.LearningPlanStatusType, new[] {"PlanStatusTypeNo", "PlanStatusTypeName"}},
                {IdpEmployeeListSortField.ApprovalGroup, new[] {"ApprovalGroups"}},
                {IdpEmployeeListSortField.UserPool, new[] {"UserPools"}},
                {IdpEmployeeListSortField.OtherUserGroup, new[] {"OtherGroups"}},
                {IdpEmployeeListSortField.CareerPath, new[] {"CareerPaths"}},
                {IdpEmployeeListSortField.DevelopmentalRole, new[] {"DevelopmentalRoles"}},
                {IdpEmployeeListSortField.ExperienceCategory, new[] {"ExperienceCategories"}},
                {IdpEmployeeListSortField.PersonnelGroup, new[] {"PersonnelGroups"}},
                {IdpEmployeeListSortField.Role, new[] {"RoleInfos"}},
                {IdpEmployeeListSortField.SystemRole, new[] {"SystemRoleInfos"}}
            };

        private readonly ILogger _logger;
        private readonly AppSettings _appSettings;

        private readonly OrganizationDbContext _organizationDbContext;
        private readonly IWorkContext _workContext;
        private readonly IUserAccessService _userAccessService;
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly IUserRepository _userRepository;


        public EmployeeListService(ILogger<EmployeeListService> logger,
            IWorkContext workContext,
            OrganizationDbContext organizationDbContext,
            IUserAccessService userAccessService,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IUserTypeRepository userTypeRepository,
            IUserRepository userRepository,
            IOptions<AppSettings> appSettingsOptions)
        {
            _logger = logger;
            _organizationDbContext = organizationDbContext;
            _workContext = workContext;
            _userAccessService = userAccessService;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _userTypeRepository = userTypeRepository;
            _userRepository = userRepository;
            _appSettings = appSettingsOptions.Value;
        }

        public async Task<IdpEmployeeListDto> GetIdpEmployeeListAsync(IdpEmployeeListArguments idpEmployeeListArgument)
        {

            UserEntity executorUser = null;
            if (!string.IsNullOrEmpty(_workContext.Sub))
            {
                executorUser = DomainHelper.GetUserEntityFromWorkContextSub(_workContext, _userRepository, true);
            }

            if (idpEmployeeListArgument.ForCurrentUser)
            {
                idpEmployeeListArgument.UserIds = new List<int> {executorUser.UserId};
            }

            var paginatedEmployeeListEntity = await GeUserListItemEntitiesFromDatabase(executorUser,
                idpEmployeeListArgument.GetPageSize(), idpEmployeeListArgument.GetPageIndex(), idpEmployeeListArgument);


            return new IdpEmployeeListDto
            {
                PageSize = paginatedEmployeeListEntity.PageSize,
                PageIndex = paginatedEmployeeListEntity.PageIndex,
                Items = paginatedEmployeeListEntity.Items.Select(ConvertToIdpEmployeeItemDto).ToList(),
                TotalItems = paginatedEmployeeListEntity.TotalItems,
                HasMoreData = paginatedEmployeeListEntity.HasMoreData
            };
        }

        private async Task<(bool Success, List<SqlParameter> SqlParameters)> BuildEmployeeListArgumentsToSqlParameter(UserEntity executorUser, int pageSize, int pageIndex,
            IdpEmployeeListArguments idpEmployeeListArgument)
        {
            var sqlParameters = new List<SqlParameter>();
            var ownerId = _workContext.CurrentOwnerId;
            var customerIds = new List<int> {_workContext.CurrentCustomerId};
            List<string> userExtIds = null;
            List<string> loginServiceClaims = null;
            List<int> userGroupIds = null;
            var userIds = idpEmployeeListArgument.UserIds;
            var parentDepartmentIds = idpEmployeeListArgument.DepartmentIds;
            var multiUserGroupFilters = idpEmployeeListArgument.MultiUserGroupIds;
            if (idpEmployeeListArgument.FilterOnSubDepartment == true && !parentDepartmentIds.IsNullOrEmpty())
            {
                _logger.LogDebug($"Start retrieving departmentIds for filtering on sub-departments.'");
                var currentHd = _hierarchyDepartmentRepository.GetById(_workContext.CurrentHdId);

                parentDepartmentIds = currentHd == null
                    ? new List<int>()
                    : _hierarchyDepartmentRepository.GetAllDepartmentIdsFromAHierachyDepartmentToBelow(
                        currentHd.HierarchyId, parentDepartmentIds);
                _logger.LogDebug(
                    $"End retrieving departmentIds for filtering on sub-departments. {parentDepartmentIds.Count} departmentIds has been retrieved.");

                if (parentDepartmentIds.IsNullOrEmpty())
                    return (false, sqlParameters);
            }

            var userAccessChecking = await _userAccessService.CheckReadUserAccessAsync(workContext: _workContext, executorUser: executorUser, ownerId: ownerId,
                customerIds: customerIds,
                userExtIds: userExtIds,
                loginServiceClaims: loginServiceClaims,
                userIds: userIds,
                userGroupIds: userGroupIds,
                parentDepartmentIds: parentDepartmentIds,
                multiUserGroupFilters: multiUserGroupFilters,
                userTypeIdsFilter: null,
                userTypeExtIdsFilter: null,
                multipleUserTypeIdsFilter: idpEmployeeListArgument.MultiUserTypeIds,
                multipleUserTypeExtIdsFilter: idpEmployeeListArgument.MultiUserTypeExtIds);

            if (!userAccessChecking.IsAllowedAccess)
            {
                return (false, sqlParameters);

            }

            userIds = userAccessChecking.UserIds;
            parentDepartmentIds = userAccessChecking.ParentDepartmentIds;
            multiUserGroupFilters = userAccessChecking.MultiUserGroupFilters;

            var multiUserTypeIdFilters = userAccessChecking.MultiUserTypeFilters;

            sqlParameters.AddMultiValuesParameter("@MultiUserTypeIdFilter", multiUserTypeIdFilters);
            sqlParameters.AddSingleValueParameter("@LanguageId", _workContext.CurrentLanguageId);
            sqlParameters.AddSingleValueParameter("@PageSize", pageSize);
            sqlParameters.AddSingleValueParameter("@PageIndex", pageIndex);
            sqlParameters.AddSingleValueParameter("@OwnerId", _workContext.CurrentOwnerId);
            sqlParameters.AddSingleValueParameter("@CustomerId", _workContext.CurrentCustomerId);
            sqlParameters.AddSingleValueParameter("@SearchKey", idpEmployeeListArgument.IdpEmployeeSearchKey);
            sqlParameters.AddSingleValueParameter("@EnableSearchingSsn", _appSettings.EnableSearchingSSN ? 1 : 0);

            if (!idpEmployeeListArgument.Genders.IsNullOrEmpty())
            {
                var genderValues = idpEmployeeListArgument.Genders.Select(g => (int) g).ToList();
                sqlParameters.AddMultiValuesParameter("@GenderFilter", genderValues);

            }

            List<int> entityStatusesIds = null;
            if (idpEmployeeListArgument.EntityStatuses.IsNullOrEmpty())
            {
                entityStatusesIds = new List<int> {(int) EntityStatusEnum.Active, (int) EntityStatusEnum.New};

            }
            else if(!idpEmployeeListArgument.EntityStatuses.Contains(EntityStatusEnum.All))
            {
                entityStatusesIds = idpEmployeeListArgument.EntityStatuses.Select(s => (int) s).ToList();

            }

            sqlParameters.AddMultiValuesParameter("@EntityStatusIdsFilter", entityStatusesIds);

            sqlParameters.AddMultiValuesParameter("@UserIdsFilter", userIds);
            sqlParameters.AddSingleValueParameter("@UserArchetypeIdsFilter", ((int) ArchetypeEnum.Employee).ToString());
            sqlParameters.AddMultiValuesParameter("@DepartmentIdsFilter", parentDepartmentIds);
            sqlParameters.AddSingleValueParameter("@AgeRangeFilter",
                GetAgePointFromAgeRange(idpEmployeeListArgument.AgeRanges));

            //For filtering on UserDynamicAttributes, between element of given values, we use logic AND, so we join them by '&&'
            sqlParameters.AddMultiValuesParameter("@JsonDynamicAttributeFilter", idpEmployeeListArgument.UserDynamicAttributes, false, "&&");

            AddResultStatusTypeFilter(idpEmployeeListArgument, sqlParameters);

            sqlParameters.AddMultiValuesParameter("@MultiUserGroupIdFilter", multiUserGroupFilters);
            sqlParameters.AddMultiValuesParameter("@DepartmentTypeIdsFilter",
                idpEmployeeListArgument.OrganizationalUnitTypeIds);
            sqlParameters.AddSingleValueParameter("@CreatedAfter", idpEmployeeListArgument.CreatedAfter);
            sqlParameters.AddSingleValueParameter("@CreatedBefore", idpEmployeeListArgument.CreatedBefore);
            sqlParameters.AddSingleValueParameter("@ExpirationDateAfter", idpEmployeeListArgument.ExpirationDateAfter);
            sqlParameters.AddSingleValueParameter("@ExpirationDateBefore",
                idpEmployeeListArgument.ExpirationDateBefore);

            if (idpEmployeeListArgument.ExternallyMastered.HasValue)
            {
                sqlParameters.AddSingleValueParameter("@Locked",
                    idpEmployeeListArgument.ExternallyMastered.Value ? 1 : 0);
            }

            sqlParameters.AddSingleValueParameter("@OrderBy", GetOrderBy(idpEmployeeListArgument));

            return (true, sqlParameters);

        }

        private string GetOrderBy(IdpEmployeeListArguments idpEmployeeListArgument)
        {
            if (!idpEmployeeListArgument.OrderBy.IsNullOrEmpty())
            {
                return TransformOrderBy(idpEmployeeListArgument.OrderBy);
            }

            return TransformOrderBy(idpEmployeeListArgument.SortField, idpEmployeeListArgument.SortOrder);
        }

        private string TransformOrderBy(IdpEmployeeListSortField sortField, SortOrder sortOrder)
        {
            if (SortFieldEnumMappings.TryGetValue(sortField, out var sortFields))
            {
                if (sortFields != null && sortFields.Length > 0)
                {
                    var order = sortOrder == SortOrder.Ascending ? "asc" : "desc";
                    return string.Join(",", sortFields.Select(f => $"{f} {order}"));
                }
            }

            return null;
        }

        private string TransformOrderBy(Dictionary<IdpEmployeeListSortField, SortOrder> inputOrderBy)
        {
            var orderByFields = new List<string>();
            foreach (var orderByExpression in inputOrderBy)
            {
                var orderBy = TransformOrderBy(orderByExpression.Key, orderByExpression.Value);
                if (!string.IsNullOrEmpty(orderBy))
                    orderByFields.Add(orderBy);
            }

            return orderByFields.Count > 0
                ? string.Join(",", orderByFields)
                : null;
        }

        private static void AddResultStatusTypeFilter(IdpEmployeeListArguments idpEmployeeListArgument,
            List<SqlParameter> sqlParameters)
        {
            if (!idpEmployeeListArgument.PDPlanActivities.IsNullOrEmpty())

            {
                foreach (var employeeListActivity in idpEmployeeListArgument.PDPlanActivities)
                {
                    var activity = employeeListActivity.Value;

                    switch (employeeListActivity.Key)
                    {

                        case PDPlanActivity.LearningNeed:
                        {


                            SqlParameterExtension.AddSingleValueParameter(sqlParameters, "@IdpNeedActivityId", activity.ActivityId);

                            SqlParameterExtension.AddSingleValueParameter(sqlParameters, "@IdpNeedDefaultStatusTypeId",
                                activity.DefaultStatusTypeId);

                            sqlParameters.AddMultiValuesParameter("@IdpNeedResultStatusTypeIdsFilter",
                                activity.StatusTypeIds);

                            sqlParameters.AddMultiValuesParameter("@IdpNeedResultAllowedStatusTypeIds",
                                activity.AllowedStatusTypeIds);

                            sqlParameters.AddMultiValuesParameter("@IdpNeedStatusTypeLogFilter",
                                BuildStatusTypeLogFilters(activity.StatusTypeLogs), false, "&&");


                                break;

                        }
                        case PDPlanActivity.LearningPlan:
                        {
                            SqlParameterExtension.AddSingleValueParameter(sqlParameters, "@IdpPlanActivityId", activity.ActivityId);

                            SqlParameterExtension.AddSingleValueParameter(sqlParameters, "@IdpPlanDefaultStatusTypeId",
                                activity.DefaultStatusTypeId);

                            sqlParameters.AddMultiValuesParameter("@IdpPlanResultStatusTypeIdsFilter",
                                activity.StatusTypeIds);

                            sqlParameters.AddMultiValuesParameter("@IdpPlanResultAllowedStatusTypeIds",
                                activity.AllowedStatusTypeIds);

                            sqlParameters.AddMultiValuesParameter("@IdpPlanStatusTypeLogFilter",
                                BuildStatusTypeLogFilters(activity.StatusTypeLogs), false, "&&");


                                break;
                        }
                    }
                }
            }

        }

        private static List<string> BuildStatusTypeLogFilters(List<StatusTypeLogFilter> statusTypeLogFilters)
        {
            if (statusTypeLogFilters.IsNullOrEmpty()) return null;
            return statusTypeLogFilters.Select(BuildStatusTypeLogFilter).Where(v => v != null).ToList();
        }
        private static string BuildStatusTypeLogFilter(StatusTypeLogFilter statusTypeLogFilter)
        {
            if (statusTypeLogFilter == null) 
                return null;

            var statusTypeIds = statusTypeLogFilter.StatusTypeIds.IsNullOrEmpty()
                    ? null
                    : string.Join(",", statusTypeLogFilter.StatusTypeIds);

            var fromDate = statusTypeLogFilter.ChangedAfter?.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var toDate = statusTypeLogFilter.ChangedBefore?.ToString("yyyy-MM-dd HH:mm:ss.fff");

            if (statusTypeIds == null && fromDate == null && toDate == null) 
                return null;
            return $"{statusTypeIds}|{fromDate}|{toDate}";
        }

        private string GetAgePointFromAgeRange(List<AgeRange> ageRanges)
        {
            if (ageRanges.IsNullOrEmpty()) return null;
            const int minAge = 0;
            const int maxAge = 200;
            const int ageStep = 9;
            const int underTwentyMaxAge = 19;
            var ageRangeAsStrings = new List<string>();

            foreach (var age in ageRanges.Distinct())
            {
                var fromAge = minAge;
                int toAge = maxAge;
                switch (age)
                {
                    case AgeRange.UnderTwenty:
                        fromAge = minAge;
                        toAge = underTwentyMaxAge;
                        break;
                    case AgeRange.Twenties:
                    case AgeRange.Thirties:
                    case AgeRange.Forties:
                        fromAge = (int) age;
                        toAge = fromAge + ageStep;

                        break;
                    case AgeRange.FiftyAndGreater:
                        fromAge = (int) age;
                        toAge = maxAge;
                        break;
                }

                ageRangeAsStrings.Add($"{fromAge}-{toAge}");
            }

            return string.Join(",", ageRangeAsStrings);
        }

        private async Task<PaginatedList<UserListItemEntity>> GeUserListItemEntitiesFromDatabase(UserEntity executorUser,
            int pageSize, int pageIndex, IdpEmployeeListArguments idpEmployeeListArgument)
        {
            try
            {
                var parameterBuilder = await BuildEmployeeListArgumentsToSqlParameter(executorUser, pageSize, pageIndex,
                        idpEmployeeListArgument);
                if (!parameterBuilder.Success)
                {
                    return new PaginatedList<UserListItemEntity>();
                }

                var parameters = parameterBuilder.SqlParameters;


                var totalRowParameter = new SqlParameter("@TotalRow", SqlDbType.Int)
                { Direction = ParameterDirection.Output };
                parameters.Insert(0, totalRowParameter);

                var data = await _organizationDbContext.ExecStoreStoredProcedureAsync<UserListItemEntity>("prc_UserList_get", parameters.ToArray());
                var total = (int)totalRowParameter.Value;

                var hasMoreData = (pageSize * pageIndex) < total;

                return new PaginatedList<UserListItemEntity>(data, pageIndex, pageSize, hasMoreData)
                {
                    TotalItems = (int)totalRowParameter.Value,

                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private IdpEmployeeItemDto ConvertToIdpEmployeeItemDto(UserListItemEntity userListItemEntity)
        {
            var itemListDto = new IdpEmployeeItemDto
            {
                Identity = new IdentityDto
                {
                    Archetype = (ArchetypeEnum) userListItemEntity.ArchetypeId,
                    Id = userListItemEntity.UserId,
                    ExtId = userListItemEntity.ExtId,
                    OwnerId = _workContext.CurrentOwnerId,
                    CustomerId = _workContext.CurrentCustomerId,
                },
                FirstName = userListItemEntity.FirstName,
                LastName = userListItemEntity.LastName,
                Email = userListItemEntity.Email,
                Department = new DepartmentInfo()
                {
                    Identity = new IdentityDto
                    {
                        Id = userListItemEntity.DepartmentId,
                        ExtId = userListItemEntity.DepartmentExtId,
                        Archetype = (ArchetypeEnum) userListItemEntity.DepartmentArchetypeId,
                        OwnerId = _workContext.CurrentOwnerId,
                        CustomerId = _workContext.CurrentCustomerId,
                    },
                    Name = userListItemEntity.DepartmentName,
                    Description = userListItemEntity.DepartmentDescription
                },
                EntityStatus = new EntityStatusDto()
                {
                    LastUpdated = userListItemEntity.LastUpdated,
                    LastUpdatedBy = userListItemEntity.LastUpdatedBy,
                    Deleted = userListItemEntity.Deleted.HasValue,
                    StatusId = (EntityStatusEnum) userListItemEntity.EntityStatusId,
                    StatusReasonId = (EntityStatusReasonEnum) userListItemEntity.EntityStatusReasonId,
                    ExternallyMastered = userListItemEntity.Locked == 1,
                    ActiveDate = userListItemEntity.EntityActiveDate,
                    ExpirationDate = userListItemEntity.EntityExpirationDate,
                    LastExternallySynchronized = userListItemEntity.LastSynchronized,
                    // EntityVersion = userListItemEntity.EntityVersion
                },
            };
            MapAssessmentInfos(itemListDto, userListItemEntity);

            MapUserTypeInfos(userListItemEntity, itemListDto);

            MapGroupInfos(userListItemEntity, itemListDto);

            MapInfoFromDynamicAttribute(userListItemEntity, itemListDto);

            return itemListDto;
        }

        private void MapInfoFromDynamicAttribute(UserListItemEntity userListItemEntity, IdpEmployeeItemDto itemListDto)
        {
            if (!string.IsNullOrEmpty(userListItemEntity.DynamicAttributes))
            {
                var jsonDynamicAttributes = new Dictionary<string, dynamic>(StringComparer.CurrentCultureIgnoreCase);
                //PopulateObject to map data to defined ignore case dictionary
                JsonConvert.PopulateObject(userListItemEntity.DynamicAttributes, jsonDynamicAttributes);

                itemListDto.AvatarUrl = GetJsonDynamicAttribute<string>(jsonDynamicAttributes, "avatarUrl");
            }
        }

        private T GetJsonDynamicAttribute<T>(Dictionary<string, dynamic> jsonDynamicAttributes, string attributeName)
        {
            if (!jsonDynamicAttributes.IsNullOrEmpty() &&
                jsonDynamicAttributes.TryGetValue(attributeName, out dynamic attribute))
            {
                return (T) attribute;
            }

            return default(T);
        }

        private void MapGroupInfos(UserListItemEntity userListItemEntity, IdpEmployeeItemDto itemListDto)
        {
            if (!string.IsNullOrEmpty(userListItemEntity.UserGroups))
            {
                var userGroupInfoEntities =
                    JsonConvert.DeserializeObject<List<UserGroupInfoEntity>>(userListItemEntity.UserGroups);
                if (!userGroupInfoEntities.IsNullOrEmpty())
                {
                    var userGroupInfos = new List<UserGroupInfo>();
                    foreach (var userGroupInfoEntity in userGroupInfoEntities)
                    {
                        var userGroupInfo = MapToUserGroupInfo(userGroupInfoEntity);
                        userGroupInfos.Add(userGroupInfo);
                    }

                    var aprrovalGroups =
                        userGroupInfos.Where(u => u.Identity.Archetype == ArchetypeEnum.ApprovalGroup).ToList();
                    var userPools =
                        userGroupInfos.Where(u => u.Identity.Archetype == ArchetypeEnum.UserPool).ToList();


                    itemListDto.UserPools = userPools;
                    itemListDto.ApprovalGroups = aprrovalGroups;
                    itemListDto.OtherUserGroups = userGroupInfos.Except(aprrovalGroups).Except(userPools).ToList();
                    itemListDto.UserGroupInfos = userGroupInfos;

                }
            }
        }

        private void MapUserTypeInfos(UserListItemEntity userListItemEntity, IdpEmployeeItemDto itemListDto)
        {
            if (!string.IsNullOrEmpty(userListItemEntity.UserTypes))
            {
                var userTypeInfoEntities =
                    JsonConvert.DeserializeObject<List<UserTypeInfoEntity>>(userListItemEntity.UserTypes);
                if (!userTypeInfoEntities.IsNullOrEmpty())
                {
                    var userTypeInfos = new List<UserTypeInfo>();
                    foreach (var userTypeInfoEntity in userTypeInfoEntities)
                    {
                        var userTypeInfo = MapObjectInfo<UserTypeInfoEntity, UserTypeInfo>(userTypeInfoEntity);
                        userTypeInfos.Add(userTypeInfo);
                    }

                    var userTypeInfosGroupedByArchetype = userTypeInfos.GroupBy(i => i.Identity.Archetype);
                    foreach (var userTypeInfosGroup in userTypeInfosGroupedByArchetype)
                    {
                        switch (userTypeInfosGroup.Key)
                        {
                            case ArchetypeEnum.CareerPath:
                                itemListDto.CareerPaths = userTypeInfosGroup.ToList();
                                break;
                            case ArchetypeEnum.LearningFramework:
                                itemListDto.LearningFrameworks = userTypeInfosGroup.ToList();
                                break;
                            case ArchetypeEnum.DevelopmentalRole:
                                itemListDto.DevelopmentalRoles = userTypeInfosGroup.ToList();
                                break;
                            case ArchetypeEnum.PersonnelGroup:
                                itemListDto.PersonnelGroups = userTypeInfosGroup.ToList();
                                break;
                            case ArchetypeEnum.ExperienceCategory:
                                itemListDto.ExperienceCategories = userTypeInfosGroup.ToList();
                                break;
                            case ArchetypeEnum.Role:
                                itemListDto.RoleInfos = userTypeInfosGroup.ToList();
                                break;
                            case ArchetypeEnum.SystemRole:
                                itemListDto.SystemRoleInfos = userTypeInfosGroup.ToList();
                                break;
                        }
                    }
                }
            }
        }

        private TObject MapObjectInfo<TEntity, TObject>(TEntity infoEntity) where TObject : ObjectBasicInfo, new()
            where TEntity : InfoEntity
        {
            return new TObject
            {
                Identity = new IdentityDto
                {
                    Archetype = (ArchetypeEnum) infoEntity.ArchetypeId,
                    ExtId = infoEntity.ExtId,
                    Id = infoEntity.Id,
                    CustomerId = _workContext.CurrentCustomerId,
                    OwnerId = _workContext.CurrentOwnerId
                },
                Name = infoEntity.Name,
                Description = infoEntity.Description
            };

        }

        private UserGroupInfo MapToUserGroupInfo(UserGroupInfoEntity infoEntity)
        {
            var userGroup = MapObjectInfo<UserGroupInfoEntity, UserGroupInfo>(infoEntity);
            userGroup.UserId = infoEntity.UserId;
            userGroup.DepartmentId = infoEntity.DepartmentId;
            userGroup.Type = (GrouptypeEnum) (infoEntity.UserGroupTypeId ?? 1);
            return userGroup;
        }

        private void MapAssessmentInfos(IdpEmployeeItemDto employeeItemDto, UserListItemEntity userListItemEntity)
        {
            var assessmentInfos = new Dictionary<PDPlanActivity, AssessmentPDPlannerInfo>
            {
                {
                    PDPlanActivity.LearningNeed, new AssessmentPDPlannerInfo
                    {
                        Identity = new IdentityDto
                        {
                            Id = userListItemEntity.NeedResultId,
                            ExtId = userListItemEntity.NeedResultExtId,
                            Archetype = ArchetypeEnum.Assessment,
                            CustomerId = _workContext.CurrentCustomerId,
                            OwnerId = _workContext.CurrentOwnerId
                        },
                        DueDate = userListItemEntity.NeedResultDueDate,
                        CompletionRate = userListItemEntity.NeedResultCompletionRate,
                        StatusInfo = new StatusTypeInfo
                        {
                            AssessmentStatusId = userListItemEntity.NeedStatusTypeId,
                            AssessmentStatusCode = userListItemEntity.NeedStatusTypeCodeName,
                            AssessmentStatusName = userListItemEntity.NeedStatusTypeName,
                            AssessmentStatusDescription = userListItemEntity.NeedStatusTypeDescription,
                            No = userListItemEntity.NeedStatusTypeNo
                        }
                    }
                },
                {
                    PDPlanActivity.LearningPlan, new AssessmentPDPlannerInfo
                    {
                        Identity = new IdentityDto
                        {
                            Id = userListItemEntity.PlanResultId,
                            ExtId = userListItemEntity.PlanResultExtId,
                            Archetype = ArchetypeEnum.Assessment,
                            CustomerId = _workContext.CurrentCustomerId,
                            OwnerId = _workContext.CurrentOwnerId
                        },
                        DueDate = userListItemEntity.PlanResultDueDate,
                        StatusInfo = new StatusTypeInfo
                        {
                            AssessmentStatusId = userListItemEntity.PlanStatusTypeId,
                            AssessmentStatusCode = userListItemEntity.PlanStatusTypeCodeName,
                            AssessmentStatusName = userListItemEntity.PlanStatusTypeName,
                            AssessmentStatusDescription = userListItemEntity.PlanStatusTypeDescription,
                            No = userListItemEntity.PlanStatusTypeNo
                        }
                    }
                }

            };
            employeeItemDto.AssessmentInfos = assessmentInfos;
        }
    }

    public class InfoEntity
    {
        public int Id { get; set; }
        public int ArchetypeId { get; set; }
        public string ExtId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UserTypeInfoEntity : InfoEntity
    {
    }

    public class UserGroupInfoEntity : InfoEntity
    {
        public int? UserId { get; set; }
        public int? DepartmentId { get; set; }
        public int? UserGroupTypeId { get; set; }
    }
}
