using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using cxOrganization.Client;
using cxOrganization.Client.UserGroups;
using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.ApiClient;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.Dtos.DataHub;
using cxOrganization.Domain.Dtos.Departments;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Entities;
using cxOrganization.Domain.Extensions;
using cxOrganization.Domain.Repositories;
using cxOrganization.Domain.Repositories.QueryBuilders;
using cxOrganization.Domain.Security.AccessServices;
using cxOrganization.Domain.Security.HierarchyDepartment;
using cxOrganization.Domain.Security.User;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using cxPlatform.Core.Extentions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace cxOrganization.Domain.Services.Reports
{
    public class UserReportService : IUserReportService
    {
        private readonly IUserService _userService;
        private readonly IUGMemberService _ugMemberService;
        //private IAdvancedWorkContext _workContext;
        private readonly IDataHubQueryApiClient _dataHubQueryApiClient;
        private readonly AppSettings _appSettings;
        private readonly IOptions<AppSettings> _appSettingOptions;
        private readonly ILogger _logger;
        private readonly IHierarchyDepartmentRepository _hierarchyDepartmentRepository;
        private readonly IUserAccessService _userAccessService;
        private readonly IUserRepository _userRepository;
        private readonly IUserCryptoService _userCryptoService;
        private readonly IUserTypeRepository _userTypeRepository;
        private readonly ILearningCatalogClientService _learningCatalogClientService;
        private readonly HierarchyDepartmentPermissionSettings _hierarchyDepartmentPermissionSettings;

        private static readonly Dictionary<UserEventType, string> EventTypeRoutingActionMapping =
            new Dictionary<UserEventType, string>()
            {
                {UserEventType.RegisteredSuccess, "cxid.system_success.register.user"},
                {UserEventType.DeletedSuccess, "cxid.system_success.delete.user"},
                {UserEventType.LoginSuccess, "cxid.system_success.login.user"},
                {UserEventType.LoginFail, "cxid.system_warn.login.user"},
                {UserEventType.LogoutSuccess, "cxid.system_success.logout.user"},
                {UserEventType.LogoutFail, "cxid.system_warn.logout.user"},
                {UserEventType.Locked, "cxid.system_warn.locked.user"},
                {UserEventType.ResetPasswordSuccess, "cxid.system_success.resetpassword.user"},
                {UserEventType.ChangePasswordSuccess, "cxid.system_success.changepassword.user"},
                {UserEventType.SetPasswordSuccess, "cxid.system_success.setpassword.user"},
                {UserEventType.ForgetPasswordSuccess, "cxid.system_success.forgotpassword.user"},
                {UserEventType.ForgetPasswordFail, "cxid.system_warn.forgotpassword.user"},
                {UserEventType.ClaimOtpEmailSuccess, "cxid.system_success.claimotpviaemail.user"},
                {UserEventType.ClaimOtpEmailFail, "cxid.system_warn.claimotpviaemail.user"},
                {UserEventType.ClaimOtpPhoneSuccess, "cxid.system_success.claimotpviaphone.user"},
                {UserEventType.ClaimOtPhoneFail, "cxid.system_warn.claimotpviaphone.user"},
                {UserEventType.Claim2faSuccess, "cxid.system_success.sendcode.user"},
                {UserEventType.Claim2faFail, "cxid.system_warn.sendcode.user"},
                {UserEventType.Enable2faSuccess, "cxid.system_success.enabletwofactorauthentication.user"},
                {UserEventType.Disable2faSuccess, "cxid.system_success.disabletwofactorauthentication.user"},
                {UserEventType.Verify2faSuccess, "cxid.system_success.verify2facode.user"},
                {UserEventType.Verify2faFail, "cxid.system_warn.verify2facode.user"}

            };
        private static readonly Dictionary<UserEventType, string> EventTypeMessageInfoMapping =
           new Dictionary<UserEventType, string>()
           {    
                {UserEventType.ForgetPasswordSuccess, "Requested forgot password successfully."},
                {UserEventType.ForgetPasswordFail, "Requested forgot password failed."},
                {UserEventType.ClaimOtpEmailSuccess, "Claim OTP via email '{Email}' successfully."},
                {UserEventType.ClaimOtpEmailFail, "Claim OTP via email '{Email}' failed."},
                {UserEventType.ClaimOtpPhoneSuccess, "Claim OTP via phone '{Phone}' successfully."},
                {UserEventType.ClaimOtPhoneFail, "Claim OTP via phone '{Phone}' failed."},
                {UserEventType.Claim2faSuccess, "Claim 2FA code successfully."},
                {UserEventType.Claim2faFail, "Claim 2FA code failed."},
                {UserEventType.Verify2faSuccess, "Verify 2FA code successfully."},
                {UserEventType.Verify2faFail, "Verify 2FA code failed."}
           };


        public UserReportService(
            ILogger<UserReportService> logger,
            IOptions<AppSettings> appSettingOptions,
            Func<ArchetypeEnum, IUserService> userService,
            IDataHubQueryApiClient dataHubQueryApiClient,
            IUGMemberService ugMemberService,
            IHierarchyDepartmentRepository hierarchyDepartmentRepository,
            IUserAccessService userAccessService,
            IUserRepository userRepository,
            IUserCryptoService userCryptoService,
            IUserTypeRepository userTypeRepository,
            ILearningCatalogClientService learningCatalogClientService,
            IOptions<HierarchyDepartmentPermissionSettings> hierarchyDepartmentPermissionSettingsOptions)
        {
            _userService = userService(ArchetypeEnum.Unknown);
            _dataHubQueryApiClient = dataHubQueryApiClient;
            _ugMemberService = ugMemberService;
            _appSettings = appSettingOptions.Value;
            _logger = logger;
            _hierarchyDepartmentRepository = hierarchyDepartmentRepository;
            _userAccessService = userAccessService;
            _userRepository = userRepository;
            _appSettingOptions = appSettingOptions;
            _userCryptoService = userCryptoService;
            _userTypeRepository = userTypeRepository;
            _learningCatalogClientService = learningCatalogClientService;
            _hierarchyDepartmentPermissionSettings = hierarchyDepartmentPermissionSettingsOptions.Value;
        }

        public async Task<List<UserEventLogInfo>> GetUserEventLogInfosAsync(IAdvancedWorkContext workContext, List<UserEventType> eventTypes,
            DateTime? eventCreatedAfter, DateTime? eventCreatedBefore, bool getDepartment, bool getRole)
        {
            const int pageSize = 500;
            var pageIndex = 1;
            var hasMoreData = true;
            List<UserEventLogInfo> items = new List<UserEventLogInfo>();
            while (hasMoreData)
            {
                var paginatedItems = await GetPaginatedUserEventLogInfosAsync(workContext, eventTypes, eventCreatedAfter,
                    eventCreatedBefore, pageSize, pageIndex, getDepartment, getRole);
                items.AddRange(paginatedItems.Items);
                hasMoreData = paginatedItems.HasMoreData;
                pageIndex++;
            }
            return items;

        }

        public async Task<int> CountUserEventAsync(IAdvancedWorkContext workContext,
            List<UserEventType> eventTypes,
            DateTime? eventCreatedAfter,
            DateTime? eventCreatedBefore)
        {
            List<string> routingActions = null;
            if (!eventTypes.IsNullOrEmpty())
            {
                routingActions = EventTypeRoutingActionMapping.Where(c => eventTypes.Contains(c.Key))
                    .Select(v => v.Value).Distinct().ToList();
            }

            return await _dataHubQueryApiClient.CountEventLogsAsync(
                new RequestContext(workContext),
                routingActions,
                null,
                eventCreatedAfter,
                eventCreatedBefore);
        }
        public async Task<PaginatedList<UserEventLogInfo>> GetPaginatedUserEventLogInfosAsync(IAdvancedWorkContext workContext,
            List<UserEventType> eventTypes, 
            DateTime? eventCreatedAfter, DateTime? eventCreatedBefore,
            int pageSize, int pageIndex, bool getDepartment, bool getRole)
        {

            List<string> routingActions = null;
            if (!eventTypes.IsNullOrEmpty())
            {
                routingActions = EventTypeRoutingActionMapping.Where(c => eventTypes.Contains(c.Key))
                    .Select(v => v.Value).Distinct().ToList();
            }

            var paginatedEvents =  await _dataHubQueryApiClient.GetPaginatedEventLogsAsync(new RequestContext(workContext),
                routingActions,
                null,
                eventCreatedAfter,
                eventCreatedBefore,
                DatahubQuerySortField.CREATED,
                DatahubQuerySortOrder.DESC,
                pageIndex, pageSize);

            if (!paginatedEvents.Items.IsNullOrEmpty())
            {
                var allEventUserIds = paginatedEvents.Items
                    .Select(p => string.IsNullOrEmpty(p.Payload?.Identity?.UserId) ? p.Routing?.EntityId : p.Payload.Identity.UserId).Distinct()
                    .Where(i => !string.IsNullOrEmpty(i)).ToList();

                var existingUserDtos = await GetUserGenericDtosAsync(workContext.CurrentOwnerId,
                    customerIds: new List<int> { workContext.CurrentCustomerId},
                    loginServiceClaims: allEventUserIds,
                    getDepartment: getDepartment,
                    getRole: getRole);

                var eventInfos = new List<UserEventLogInfo>();
                foreach (GenericLogEventMessage logEventMessage in paginatedEvents.Items)
                {
                    var eventCreatedByUserId = logEventMessage.Payload?.Identity?.UserId;
                    if (string.IsNullOrEmpty(eventCreatedByUserId))
                    {
                        eventCreatedByUserId = logEventMessage.Routing?.EntityId;
                    }

                    var user = FindUser(existingUserDtos, eventCreatedByUserId, true);
                    var eventInfo = GenerateEventLogInfo(logEventMessage, user, eventCreatedByUserId);
                    eventInfos.Add(eventInfo);
                }

                return new PaginatedList<UserEventLogInfo>(eventInfos,
                    paginatedEvents.PageIndex,
                    paginatedEvents.PageSize,
                    paginatedEvents.HasMoreData)
                {
                    TotalItems = paginatedEvents.TotalItems

                };
            }

            return new PaginatedList<UserEventLogInfo>();
        }

        public async Task<UserStatisticsDto> GetUserStatisticsAsync(IAdvancedWorkContext workContext,
            List<EntityStatusEnum> accountStatisticsEntityStatuses,
            List<UserEventType> eventStatisticsTypes,
            bool getOnBoardingStatistics,
            DateTime? fromDate, DateTime? toDate)
        {
            var userStatistics = new UserStatisticsDto();
            if (accountStatisticsEntityStatuses.IsNullOrEmpty()
                && eventStatisticsTypes.IsNullOrEmpty()
                && !getOnBoardingStatistics)
            {
                return userStatistics;
            }

            if (!accountStatisticsEntityStatuses.IsNullOrEmpty())
            {
                userStatistics.AccountStatistics = await CalculateAccountStatisticsAsync(workContext, accountStatisticsEntityStatuses, fromDate, toDate);
            }

            if (getOnBoardingStatistics)
            {
                userStatistics.OnBoardingStatistics = await CalculateOnBoardingStatisticsAsync(workContext, fromDate, toDate);
            }

            if (!eventStatisticsTypes.IsNullOrEmpty())
            {

                userStatistics.EventStatistics = await CalculateUserEventStatisticsAsync(workContext, eventStatisticsTypes, fromDate, toDate);
            }

            return userStatistics;
        }

        private async Task<UserEventStatisticsDto> CalculateUserEventStatisticsAsync(IAdvancedWorkContext workContext, List<UserEventType> eventStatisticsTypes,
            DateTime? fromDate,
            DateTime? toDate)
        {
            var userEventStatistics = new UserEventStatisticsDto();
            var userEventsInfos = await GetUserEventLogInfosAsync(workContext, eventStatisticsTypes, fromDate, toDate, false, false);

            foreach (var eventStatisticsType in eventStatisticsTypes)
            {
                var userEventsOfType = userEventsInfos
                    .Where(e => e.Type == eventStatisticsType && e?.CreatedByUser?.Identity?.Id > 0).ToList();

                var externalMasteredUserEvents = userEventsOfType
                    .Where(e => e.CreatedByUser.EntityStatus.ExternallyMastered).ToList();
                var externalMasteredUniqueUserEventsCount= externalMasteredUserEvents.GroupBy(g => g.CreatedByUser.Identity.Id.Value).Count();
                var externalMasteredUserEventsCount = externalMasteredUserEvents.Count;

                var nonExternalMasteredUserEvents = userEventsOfType
                    .Where(e => !e.CreatedByUser.EntityStatus.ExternallyMastered).ToList();
                var nonExternalMasteredUniqueUserEvents = nonExternalMasteredUserEvents
                    .GroupBy(g => g.CreatedByUser.Identity.Id.Value).Count();
                var nonExternalMasteredUserEventsCount = nonExternalMasteredUserEvents.Count;

                userEventStatistics.Add(eventStatisticsType,
                    new Dictionary<AccountType, Dictionary<EventValueType, int>>()
                    {
                        {
                            AccountType.All, new Dictionary<EventValueType, int>
                            {
                                {
                                    EventValueType.NumberOfEvents,
                                    externalMasteredUserEventsCount + nonExternalMasteredUserEventsCount
                                },
                                {
                                    EventValueType.NumberOfUniqueUsers,
                                    externalMasteredUniqueUserEventsCount + nonExternalMasteredUniqueUserEvents
                                },
                            }
                        },
                        {
                            AccountType.ExternalMastered, new Dictionary<EventValueType, int>
                            {
                                {EventValueType.NumberOfEvents, externalMasteredUserEventsCount},
                                {EventValueType.NumberOfUniqueUsers, externalMasteredUniqueUserEventsCount},

                            }
                        },
                        {
                            AccountType.NonExternalMastered, new Dictionary<EventValueType, int>
                            {
                                {EventValueType.NumberOfEvents, nonExternalMasteredUserEventsCount},
                                {EventValueType.NumberOfUniqueUsers, nonExternalMasteredUniqueUserEvents},
                            }
                        }
                    });
            }
            return userEventStatistics;
        }

        private async Task<UserOnBoardingStatisticsDto> CalculateOnBoardingStatisticsAsync(IAdvancedWorkContext workContext, DateTime? fromDate, DateTime? toDate)
        {
            var userOnBoardingStatistics = new UserOnBoardingStatisticsDto
            {
                NotStarted = await CountUserNotStartedOnBoardingUser(workContext, fromDate, toDate),
                Started = await CountUserStartedOnBoardingUser(workContext, fromDate, toDate),
                Completed = await CountUserFinishedOnBoardingUser(workContext, fromDate, toDate)
            };

            return userOnBoardingStatistics;
        }

        public async Task<PaginatedList<ApprovingOfficerInfo>> GetPaginatedApprovingOfficerInfosAsync(
            IAdvancedWorkContext workContext,
            List<int> parentDepartmentIds,
            bool filterOnSubDepartment,
            bool getRole,
            bool getDepartment,
            DateTime? userCreatedAfter,
            DateTime? userCreatedBefore,
            DateTime? countMemberCreatedAfter,
            DateTime? countMemberCreatedBefore,            
            List<EntityStatusEnum> userEntityStatuses,
            int pageSize,
            int pageIndex)
        {
            if (_appSettings.ApprovingOfficerUserTypeExtIds.IsNullOrEmpty())
            {
                _logger.LogWarning("There is no configuration of ApprovingOfficerUserTypeExtIds to be able to get approving officers.");
                return new PaginatedList<ApprovingOfficerInfo>();
            }
            userEntityStatuses = userEntityStatuses ?? new List<EntityStatusEnum>();
            if (userEntityStatuses.IsNullOrEmpty())
            {
                userEntityStatuses.Add(EntityStatusEnum.Active);
                userEntityStatuses.Add(EntityStatusEnum.New);

            }
            var ownerId = workContext.CurrentOwnerId;
            var customerIds = new List<int> { workContext.CurrentCustomerId };

            var paginatedUsers = await _userService.GetUsersAsync<UserGenericDto>(ownerId: ownerId,
                customerIds: customerIds,
                loginServiceClaims: null,
                includeDepartment: getDepartment,
                getRoles: getRole,
                createdAfter: userCreatedAfter,
                createdBefore: userCreatedBefore,
                userTypeExtIds: _appSettings.ApprovingOfficerUserTypeExtIds,
                parentDepartmentIds: parentDepartmentIds,
                filterOnSubDepartment: filterOnSubDepartment,
                filterOnParentHd: false,
                includeOwnUserGroups: true,
                statusIds: userEntityStatuses,
                pageSize: pageSize,
                pageIndex: pageIndex);

            var approvingOfficers = new List<ApprovingOfficerInfo>();

            if (paginatedUsers.Items.Count > 0)
            {
                var approvalUserGroupIds = paginatedUsers.Items.SelectMany(u =>
                        (u.OwnGroups ?? new List<UserGroupDto>())
                        .Where(g => g.Identity.Archetype == ArchetypeEnum.ApprovalGroup)
                        .Select(g => (int)g.Identity.Id.Value))
                    .ToList();
                var splittedApprovalUserGroupIds = approvalUserGroupIds.Split(500);//To prevent filter with large list
                var countAllMemberByUserGroup = new List<KeyValuePair<int, int>>();
                foreach (var approvalUserGroupIdGroup in splittedApprovalUserGroupIds)
                {
                    var countMemberByUserGroup = await _ugMemberService.CountMemberGroupByUserGroupAsync(ownerId: ownerId,
                        customerIds: customerIds,
                        userGroupIds: approvalUserGroupIdGroup,
                        createdAfter: countMemberCreatedAfter,
                        createdBefore: countMemberCreatedBefore
                        );
                    countAllMemberByUserGroup.AddRange(countMemberByUserGroup.ToList());
                }
                foreach (var userGenericDto in paginatedUsers.Items)
                {
                    var primaryApprovalMemberCount = 0;
                    var alternativeApprovalMemberCount = 0;

                    if (userGenericDto.OwnGroups != null && countAllMemberByUserGroup.Count > 0)
                    {
                        primaryApprovalMemberCount = CountMemberInApprovalGroups(userGenericDto, GrouptypeEnum.PrimaryApprovalGroup, countAllMemberByUserGroup);
                        alternativeApprovalMemberCount = CountMemberInApprovalGroups(userGenericDto, GrouptypeEnum.AlternativeApprovalGroup, countAllMemberByUserGroup);
                    }

                    var approvingOfficer = MapToApprovingOfficerInfo(userGenericDto, primaryApprovalMemberCount, alternativeApprovalMemberCount);
                    approvingOfficers.Add(approvingOfficer);
                }

            }

            return new PaginatedList<ApprovingOfficerInfo>(approvingOfficers,
                paginatedUsers.PageIndex,
                paginatedUsers.PageSize, 
                paginatedUsers.HasMoreData);

        }


        public async Task<List<ApprovingOfficerInfo>> GetApprovingOfficerInfosAsync(
            IAdvancedWorkContext workContext,
            List<int> parentDepartmentIds,
            bool filterOnSubDepartment,
            bool getRole, bool getDepartment,
            DateTime? userCreatedAfter,
            DateTime? userCreatedBefore,
            DateTime? countMemberCreatedAfter,
            DateTime? countMemberCreatedBefore,
            List<EntityStatusEnum> userEntityStatuses)
        {
            var approvingOfficerInfos = new List<ApprovingOfficerInfo>();
            var hasMoreData = true;
            int pageIndex = 1;
            do
            {
                var paginatedUserEntities = await GetPaginatedApprovingOfficerInfosAsync(
                    workContext: workContext,
                    parentDepartmentIds: parentDepartmentIds,
                    filterOnSubDepartment: filterOnSubDepartment,
                    getRole: getRole,
                    getDepartment: getDepartment,
                    userCreatedAfter: userCreatedAfter,
                    userCreatedBefore: userCreatedBefore,
                    countMemberCreatedAfter: countMemberCreatedAfter,
                    countMemberCreatedBefore: countMemberCreatedBefore,
                    userEntityStatuses: userEntityStatuses,
                    pageIndex: pageIndex,
                    pageSize: 0);

                hasMoreData = paginatedUserEntities.HasMoreData;
                pageIndex++;
                if (paginatedUserEntities.Items.Count > 0)
                {
                    approvingOfficerInfos.AddRange(paginatedUserEntities.Items);
                }

            } while (hasMoreData);

            return approvingOfficerInfos;
        }

        public async Task<List<UserAccountDetailsInfo>> GetUserAccountDetailsInfosAsync(
            IAdvancedWorkContext workContext,
            List<int> parentDepartmentIds,
            bool? filterOnSubDepartment = null,
            List<EntityStatusEnum> userEntityStatuses = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            DateTime? entityActiveDateBefore = null,
            DateTime? firstLoginAfter = null,
            DateTime? firstLoginBefore = null,
            DateTime? lastLoginAfter = null,
            DateTime? lastLoginBefore = null,
            DateTime? onboardingAfter = null,
            DateTime? onboardingBefore = null,
            string orderBy = null)
        {
            var userAccountDetailsInfos = new List<UserAccountDetailsInfo>();
            var hasMoreData = true;
            int pageIndex = 1;
            do
            {
                var paginatedUserEntities = await GetPaginatedUserAccountDetailsInfosAsync(
                    workContext: workContext,
                    parentDepartmentIds: parentDepartmentIds,
                    filterOnSubDepartment: filterOnSubDepartment,
                    userEntityStatuses: userEntityStatuses,
                    createdAfter: createdAfter, 
                    createdBefore: createdBefore,
                    lastUpdatedAfter: lastUpdatedAfter, 
                    lastUpdatedBefore: lastUpdatedBefore,
                    expirationDateAfter: expirationDateAfter, 
                    expirationDateBefore: expirationDateBefore,
                    entityActiveDateAfter: entityActiveDateAfter,
                    entityActiveDateBefore: entityActiveDateBefore,
                    firstLoginAfter: firstLoginAfter, 
                    firstLoginBefore: firstLoginBefore,
                    lastLoginAfter: lastLoginAfter,
                    lastLoginBefore: lastLoginBefore,
                    onboardingAfter: onboardingAfter,
                    onboardingBefore: onboardingBefore,
                    pageSize: 0,
                    pageIndex: pageIndex,
                    orderBy: orderBy);

                hasMoreData = paginatedUserEntities.HasMoreData;
                pageIndex++;
                if (paginatedUserEntities.Items.Count > 0)
                {
                    userAccountDetailsInfos.AddRange(paginatedUserEntities.Items);
                }

            } while (hasMoreData);

            return userAccountDetailsInfos;
        }

        public async Task<PaginatedList<UserAccountDetailsInfo>> GetPaginatedUserAccountDetailsInfosAsync(
            IAdvancedWorkContext workContext,
            List<int> parentDepartmentIds,
            bool? filterOnSubDepartment = null,
            List<EntityStatusEnum> userEntityStatuses = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            DateTime? entityActiveDateBefore = null,
            DateTime? firstLoginAfter = null,
            DateTime? firstLoginBefore = null,
            DateTime? lastLoginAfter = null,
            DateTime? lastLoginBefore = null,
            DateTime? onboardingAfter = null,
            DateTime? onboardingBefore = null,
            int pageSize = 0,
            int pageIndex = 0, 
            string orderBy = null)
        {
            var paginatedUserAccountDetailsEntities = await GetPaginatedUserAccountDetailsEntitiesAsync(
                workContext: workContext,
                parentDepartmentIds: parentDepartmentIds,
                filterOnSubDepartment: filterOnSubDepartment,
                userEntityStatuses: userEntityStatuses,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                expirationDateAfter: expirationDateAfter,
                expirationDateBefore: expirationDateBefore,
                entityActiveDateAfter: entityActiveDateAfter,
                entityActiveDateBefore: entityActiveDateBefore,
                firstLoginAfter: firstLoginAfter,
                firstLoginBefore: firstLoginBefore,
                lastLoginAfter: lastLoginAfter,
                lastLoginBefore: lastLoginBefore,
                onboardingAfter: onboardingAfter,
                onboardingBefore: onboardingBefore,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy);

            List<UserAccountDetailsInfo> userAccountDetailsInfos;
            if (paginatedUserAccountDetailsEntities.Items.Count > 0)
            {

                var userTypeOnMemory = await _userTypeRepository.GetAllUserTypesInCacheAsync();
                var personnelGroupInfos = ExtractUserTypeInfo(workContext.CurrentLanguageId, userTypeOnMemory,
                    ArchetypeEnum.PersonnelGroup);
                var systemRoleInfos = ExtractUserTypeInfo(workContext.CurrentLanguageId, userTypeOnMemory,
                    ArchetypeEnum.SystemRole);

                var designationCatalog =
                    await _learningCatalogClientService.GetDesignations(workContext.CorrelationId);
                var orgUnitTypes =
                    await _learningCatalogClientService.GetOrganizationUnitTypes(workContext.CorrelationId);

                userAccountDetailsInfos = paginatedUserAccountDetailsEntities.Items.Select(i =>
                        MapToUserAccountDetailsInfo(i, designationCatalog, personnelGroupInfos, systemRoleInfos,
                            orgUnitTypes))
                    .ToList();
            }
            else
            {
                userAccountDetailsInfos = new List<UserAccountDetailsInfo>();
            }

            return new PaginatedList<UserAccountDetailsInfo>(userAccountDetailsInfos, pageIndex, pageSize,
                    paginatedUserAccountDetailsEntities.HasMoreData)
            {
                TotalItems = paginatedUserAccountDetailsEntities.TotalItems
            };

        }

        private UserAccountDetailsInfo MapToUserAccountDetailsInfo(UserAccountDetailsEntity item,
            List<CatalogItemDto> designationCatalog, List<(int UserTypeId, string Name)> personnelGroupInfos,
            List<(int UserTypeId, string Name)> systemRoleInfos, List<CatalogItemDto> orgUnitTypes)
        {
            var isExternallyMasteredUser = item.Locked == 1;

            return new UserAccountDetailsInfo
            {
                UserId = item.UserId,
                ExtId = item.ExtId,
                DepartmentId = item.DepartmentId,
                DepartmentName = item.DepartmentName,
                FullName = $"{item.FirstName} {item.LastName}".Trim(),
                LastLoginDate = !string.IsNullOrEmpty(item.LastLoginDate)
                    ? DateTime.Parse(item.LastLoginDate)
                    : (DateTime?)null,
                DateOnboarded = !string.IsNullOrEmpty(item.DateOnboarded)
                    ? DateTime.Parse(item.DateOnboarded)
                    : (DateTime?)null,
                AccountStatus = item.AccountStatus,
                Designation = isExternallyMasteredUser
                    ? item.JobTitle
                    : FindCatalogItemDto(designationCatalog, item.DesignationId)?.DisplayText,
                EmailAddress = item.EmailAddress,
                OnboardingStatus =
                    string.Equals(true.ToString(), item.FinishedOnBoarding, StringComparison.CurrentCultureIgnoreCase)
                        ? "Yes"
                        : "No",
                ServiceScheme = GetUserTypeNames(item.UtuEntities, personnelGroupInfos),
                SystemRoles = GetUserTypeNames(item.UtuEntities, systemRoleInfos),
                TypeOfOrganization = FindCatalogItemDto(orgUnitTypes, item.TypeOfOrganizationUnitId)?.DisplayText,
                AccountTypeDisplayText = isExternallyMasteredUser
                    ? _appSettings.ExternallyMasteredUserReportDisplayText
                    : _appSettings.NonExternallyMasteredUserReportDisplayText,
                AccountType = isExternallyMasteredUser ?
                    AccountType.ExternalMastered :
                    AccountType.NonExternalMastered
            };
        }
        
        private async Task<PagingResult<UserAccountDetailsEntity>> GetPaginatedUserAccountDetailsEntitiesAsync(IAdvancedWorkContext workContext,
            List<int> parentDepartmentIds, bool? filterOnSubDepartment,
            List<EntityStatusEnum> userEntityStatuses, DateTime? createdAfter, DateTime? createdBefore,
            DateTime? lastUpdatedAfter,
            DateTime? lastUpdatedBefore, DateTime? expirationDateAfter, DateTime? expirationDateBefore,
            DateTime? entityActiveDateAfter, DateTime? entityActiveDateBefore, DateTime? firstLoginAfter,
            DateTime? firstLoginBefore, DateTime? lastLoginAfter, DateTime? lastLoginBefore, DateTime? onboardingAfter,
            DateTime? onboardingBefore, int pageSize, int pageIndex, string orderBy)
        {
            var query = await BuildGetUserQuery(workContext: workContext, parentDepartmentIds: parentDepartmentIds,
                filterOnSubDepartment: filterOnSubDepartment,
                userEntityStatuses: userEntityStatuses,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                expirationDateAfter: expirationDateAfter,
                expirationDateBefore: expirationDateBefore,
                entityActiveDateAfter: entityActiveDateAfter,
                entityActiveDateBefore: entityActiveDateBefore,
                firstLoginAfter: firstLoginAfter,
                firstLoginBefore: firstLoginBefore,
                lastLoginAfter: lastLoginAfter,
                lastLoginBefore: lastLoginBefore,
                onboardingAfter: onboardingAfter,
                onboardingBefore: onboardingBefore, 
                userTypeIds: null);

            if (query == null) return new PagingResult<UserAccountDetailsEntity>();

            var designationPath = $"$.{UserJsonDynamicAttributeName.Designation}";
            var jobTittlePath = $"$.{UserJsonDynamicAttributeName.JobTitle}";

            var lastLoginPath = $"$.{UserJsonDynamicAttributeName.LastLoginDate}";
            var onBoardingDatePath = $"$.{UserJsonDynamicAttributeName.FinishedOnboardingDate}";
            var finishOnBoardingPath = $"$.{UserJsonDynamicAttributeName.FinishOnBoarding}";
            var typeOfOrganizationUnitsPath = $"$.{DepartmentJsonDynamicAttributeName.TypeOfOrganizationUnit}";

            if (!string.IsNullOrEmpty(orderBy))
            {
                query = query.ApplyOrderBy(p => p.UserId, orderBy);
            }
            var selectQuery = query.Select(userEntity => new UserAccountDetailsEntity
            {
                UserId = userEntity.UserId,
                ExtId = userEntity.ExtId,
                EmailAddress = userEntity.Email,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                DepartmentId = userEntity.DepartmentId,
                DepartmentName = userEntity.Department.Name,
                DesignationId = EfJsonExtensions.JsonValue(userEntity.DynamicAttributes, designationPath),
                JobTitle = EfJsonExtensions.JsonValue(userEntity.DynamicAttributes, jobTittlePath),
                LastLoginDate = EfJsonExtensions.JsonValue(userEntity.DynamicAttributes, lastLoginPath),
                DateOnboarded = EfJsonExtensions.JsonValue(userEntity.DynamicAttributes, onBoardingDatePath),
                AccountStatus = DomainHelper.MapEntityStatusToUserStatus((EntityStatusEnum)userEntity.EntityStatusId),
                FinishedOnBoarding = EfJsonExtensions.JsonValue(userEntity.DynamicAttributes, finishOnBoardingPath),
                TypeOfOrganizationUnitId = EfJsonExtensions.JsonValue(userEntity.Department.DynamicAttributes, typeOfOrganizationUnitsPath),
                UtuEntities = userEntity.UT_Us,
                Locked = userEntity.Locked
            }); ;
            return await selectQuery.ToPagingAsync(pageIndex, pageSize);
        }


        public async Task<List<PrivilegedUserAccountInfo>> GetPrivilegedUserAccountInfosAsync(
            IAdvancedWorkContext workContext,
            List<int> parentDepartmentIds,
            bool? filterOnSubDepartment = null,
            List<EntityStatusEnum> userEntityStatuses = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            DateTime? entityActiveDateBefore = null,
            DateTime? firstLoginAfter = null,
            DateTime? firstLoginBefore = null,
            DateTime? lastLoginAfter = null,
            DateTime? lastLoginBefore = null,
            DateTime? onboardingAfter = null,
            DateTime? onboardingBefore = null,
            string orderBy = null,
            bool? needDepartmentPathName = null)
        {
            var userAccountDetailsInfos = new List<PrivilegedUserAccountInfo>();
            var hasMoreData = true;
            int pageIndex = 1;
            do
            {
                var paginatedUserEntities = await GetPaginatedPrivilegedUserAccountInfosAsync(
                    workContext: workContext,
                    parentDepartmentIds: parentDepartmentIds,
                    filterOnSubDepartment: filterOnSubDepartment,
                    userEntityStatuses: userEntityStatuses,
                    createdAfter: createdAfter,
                    createdBefore: createdBefore,
                    lastUpdatedAfter: lastUpdatedAfter,
                    lastUpdatedBefore: lastUpdatedBefore,
                    expirationDateAfter: expirationDateAfter,
                    expirationDateBefore: expirationDateBefore,
                    entityActiveDateAfter: entityActiveDateAfter,
                    entityActiveDateBefore: entityActiveDateBefore,
                    firstLoginAfter: firstLoginAfter,
                    firstLoginBefore: firstLoginBefore,
                    lastLoginAfter: lastLoginAfter,
                    lastLoginBefore: lastLoginBefore,
                    onboardingAfter: onboardingAfter,
                    onboardingBefore: onboardingBefore,
                    pageSize: 0,
                    pageIndex: pageIndex,
                    orderBy: orderBy,
                    needDepartmentPathName: needDepartmentPathName);

                hasMoreData = paginatedUserEntities.HasMoreData;
                pageIndex++;
                if (paginatedUserEntities.Items.Count > 0)
                {
                    userAccountDetailsInfos.AddRange(paginatedUserEntities.Items);
                }

            } while (hasMoreData);

            return userAccountDetailsInfos;
        }
        public async Task<PaginatedList<PrivilegedUserAccountInfo>> GetPaginatedPrivilegedUserAccountInfosAsync(
            IAdvancedWorkContext workContext,
            List<int> parentDepartmentIds,
            bool? filterOnSubDepartment = null,
            List<EntityStatusEnum> userEntityStatuses = null,
            DateTime? createdAfter = null,
            DateTime? createdBefore = null,
            DateTime? lastUpdatedAfter = null,
            DateTime? lastUpdatedBefore = null,
            DateTime? expirationDateAfter = null,
            DateTime? expirationDateBefore = null,
            DateTime? entityActiveDateAfter = null,
            DateTime? entityActiveDateBefore = null,
            DateTime? firstLoginAfter = null,
            DateTime? firstLoginBefore = null,
            DateTime? lastLoginAfter = null,
            DateTime? lastLoginBefore = null,
            DateTime? onboardingAfter = null,
            DateTime? onboardingBefore = null,
            int pageSize = 0,
            int pageIndex = 0,
            string orderBy = null,
            bool? needDepartmentPathName = null)
        {
            var userTypeOnMemory = await _userTypeRepository.GetAllUserTypesInCacheAsync();

            var privilegedUserTypeExtIds = _appSettings.PrivilegedUserTypeExtIds;
            if (privilegedUserTypeExtIds.IsNullOrEmpty())
            {
                _logger.LogWarning("Unable to get privileged user since missing configuration of PrivilegedUserTypeExtIds");
                return new PaginatedList<PrivilegedUserAccountInfo>();
            }

            var privilegedUserTypeIds = userTypeOnMemory
                .Where(ut =>
                    ut.ArchetypeId == (int) ArchetypeEnum.SystemRole &&
                    privilegedUserTypeExtIds.Contains(ut.ExtId, StringComparer.CurrentCultureIgnoreCase))
                .Select(u => u.UserTypeId).ToList();

            if (privilegedUserTypeIds.Count == 0)
            {
                return new PaginatedList<PrivilegedUserAccountInfo>();
            }

            var paginatedPrivilegedUserAccountEntities = await GetPaginatedPrivilegedUserAccountEntitiesAsync(workContext: workContext,
                parentDepartmentIds: parentDepartmentIds,
                filterOnSubDepartment: filterOnSubDepartment,
                userEntityStatuses: userEntityStatuses,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                expirationDateAfter: expirationDateAfter,
                expirationDateBefore: expirationDateBefore,
                entityActiveDateAfter: entityActiveDateAfter,
                entityActiveDateBefore: entityActiveDateBefore,
                firstLoginAfter: firstLoginAfter,
                firstLoginBefore: firstLoginBefore,
                lastLoginAfter: lastLoginAfter,
                lastLoginBefore: lastLoginBefore,
                onboardingAfter: onboardingAfter,
                onboardingBefore: onboardingBefore,
                pageSize: pageSize,
                pageIndex: pageIndex,
                orderBy: orderBy,
                privilegedUserTypeIds: privilegedUserTypeIds);

            List<PrivilegedUserAccountInfo> userAccountDetailsInfos;
            if (paginatedPrivilegedUserAccountEntities.Items.Count > 0)
            {

                var systemRoleInfos = ExtractUserTypeInfo(workContext.CurrentLanguageId, userTypeOnMemory, ArchetypeEnum.SystemRole);

                var designationCatalog =
                    await _learningCatalogClientService.GetDesignations(workContext.CorrelationId);
                var orgUnitTypes =
                    await _learningCatalogClientService.GetOrganizationUnitTypes(workContext.CorrelationId);
                Dictionary<int, string> hierarchyPathNamesGroupByDepartment= null;
                if (needDepartmentPathName == true)
                {
                    var departmentIds = paginatedPrivilegedUserAccountEntities.Items.Select(i => i.DepartmentId).Distinct().ToList();
                    if (departmentIds.Count > 0)
                    {
                        var currentHd = _hierarchyDepartmentRepository.GetById(workContext.CurrentHdId);
                        var hierarchyDepartmentEntities = _hierarchyDepartmentRepository.GetHierarchyDepartmentEntities(workContext.CurrentOwnerId, currentHd.HierarchyId, departmentIds: departmentIds);
                      
                        hierarchyPathNamesGroupByDepartment = hierarchyDepartmentEntities.ToDictionary(h => h.DepartmentId, h => h.PathName);
                        if (_hierarchyDepartmentPermissionSettings.ExcludeTheRootDepartment)
                        {
                            var keysAsDepartmentIds = hierarchyPathNamesGroupByDepartment.Keys.ToList();
                            foreach (var departmentId in keysAsDepartmentIds)
                            {
                                var originalPathName = hierarchyPathNamesGroupByDepartment[departmentId];
                                var excludedRootPathName = originalPathName.Remove(0, originalPathName.IndexOf('\\')+1);
                                hierarchyPathNamesGroupByDepartment[departmentId] = excludedRootPathName;
                            }
                        }
                     
                          
                    }
                }
                hierarchyPathNamesGroupByDepartment = hierarchyPathNamesGroupByDepartment ?? new Dictionary<int, string>();

                userAccountDetailsInfos = paginatedPrivilegedUserAccountEntities.Items
                    .Select(i => MapToPrivilegedUserAccountInfo(i, designationCatalog, systemRoleInfos, orgUnitTypes, hierarchyPathNamesGroupByDepartment))
                    .ToList();
            }
            else
            {
                userAccountDetailsInfos = new List<PrivilegedUserAccountInfo>();
            }

            return new PaginatedList<PrivilegedUserAccountInfo>(userAccountDetailsInfos, pageIndex, pageSize,
                    paginatedPrivilegedUserAccountEntities.HasMoreData)
            {
                TotalItems = paginatedPrivilegedUserAccountEntities.TotalItems
            };

        }

        private async Task<PagingResult<PrivilegedUserAccountEntity>> GetPaginatedPrivilegedUserAccountEntitiesAsync(
            IAdvancedWorkContext workContext, List<int> parentDepartmentIds, bool? filterOnSubDepartment,
            List<EntityStatusEnum> userEntityStatuses, DateTime? createdAfter, DateTime? createdBefore,
            DateTime? lastUpdatedAfter,
            DateTime? lastUpdatedBefore, DateTime? expirationDateAfter, DateTime? expirationDateBefore,
            DateTime? entityActiveDateAfter, DateTime? entityActiveDateBefore, DateTime? firstLoginAfter,
            DateTime? firstLoginBefore, DateTime? lastLoginAfter, DateTime? lastLoginBefore, DateTime? onboardingAfter,
            DateTime? onboardingBefore, int pageSize, int pageIndex, string orderBy,
            List<int> privilegedUserTypeIds)
        {
            var query = await BuildGetUserQuery(workContext:workContext,
                parentDepartmentIds: parentDepartmentIds,
                filterOnSubDepartment: filterOnSubDepartment,
                userEntityStatuses: userEntityStatuses,
                createdAfter: createdAfter,
                createdBefore: createdBefore,
                lastUpdatedAfter: lastUpdatedAfter,
                lastUpdatedBefore: lastUpdatedBefore,
                expirationDateAfter: expirationDateAfter,
                expirationDateBefore: expirationDateBefore,
                entityActiveDateAfter: entityActiveDateAfter,
                entityActiveDateBefore: entityActiveDateBefore,
                firstLoginAfter: firstLoginAfter,
                firstLoginBefore: firstLoginBefore,
                lastLoginAfter: lastLoginAfter,
                lastLoginBefore: lastLoginBefore,
                onboardingAfter: onboardingAfter,
                onboardingBefore: onboardingBefore,
                userTypeIds: privilegedUserTypeIds);

            if (query == null) return new PagingResult<PrivilegedUserAccountEntity>();

            var designationPath = $"$.{UserJsonDynamicAttributeName.Designation}";
            var jobTittlePath = $"$.{UserJsonDynamicAttributeName.JobTitle}";

            var lastLoginPath = $"$.{UserJsonDynamicAttributeName.LastLoginDate}";
            var typeOfOrganizationUnitsPath = $"$.{DepartmentJsonDynamicAttributeName.TypeOfOrganizationUnit}";

            if (!string.IsNullOrEmpty(orderBy))
            {
                query = query.ApplyOrderBy(p => p.UserId, orderBy);
            }

            var selectQuery = query.Select(userEntity => new PrivilegedUserAccountEntity
            {
                UserId = userEntity.UserId,
                ExtId = userEntity.ExtId,
                EmailAddress = userEntity.Email,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                DepartmentId = userEntity.DepartmentId,
                DesignationId = EfJsonExtensions.JsonValue(userEntity.DynamicAttributes, designationPath),
                JobTitle = EfJsonExtensions.JsonValue(userEntity.DynamicAttributes, jobTittlePath),
                LastLoginDate = EfJsonExtensions.JsonValue(userEntity.DynamicAttributes, lastLoginPath),
                TypeOfOrganizationUnitId = EfJsonExtensions.JsonValue(userEntity.Department.DynamicAttributes,
                    typeOfOrganizationUnitsPath),
                UtuEntities = userEntity.UT_Us,
                Locked = userEntity.Locked,
                Created = userEntity.Created
            });


            return await selectQuery.ToPagingAsync(pageIndex, pageSize);
        }

        private async Task<IQueryable<UserEntity>> BuildGetUserQuery(IAdvancedWorkContext workContext, List<int> parentDepartmentIds, 
            bool? filterOnSubDepartment,  List<EntityStatusEnum> userEntityStatuses,
            DateTime? createdAfter,
            DateTime? createdBefore, 
            DateTime? lastUpdatedAfter, 
            DateTime? lastUpdatedBefore, 
            DateTime? expirationDateAfter, 
            DateTime? expirationDateBefore, 
            DateTime? entityActiveDateAfter,
            DateTime? entityActiveDateBefore,
            DateTime? firstLoginAfter,
            DateTime? firstLoginBefore, 
            DateTime? lastLoginAfter, 
            DateTime? lastLoginBefore, 
            DateTime? onboardingAfter, 
            DateTime? onboardingBefore,
            List<int> userTypeIds)
        {
            userEntityStatuses = userEntityStatuses ?? new List<EntityStatusEnum>();
            if (userEntityStatuses.IsNullOrEmpty())
            {
                userEntityStatuses.Add(EntityStatusEnum.Active);
                userEntityStatuses.Add(EntityStatusEnum.New);
            }

            if (filterOnSubDepartment == true && !parentDepartmentIds.IsNullOrEmpty())
            {
                _logger.LogDebug($"Start retrieving departmentIds for filtering on sub-departments.'");
                var currentHd = await _hierarchyDepartmentRepository.GetByIdAsync(workContext.CurrentHdId);

                parentDepartmentIds = currentHd == null
                    ? new List<int>()
                    : await _hierarchyDepartmentRepository.GetAllDepartmentIdsFromAHierachyDepartmentToBelowAsync(
                        currentHd.HierarchyId, parentDepartmentIds);
                _logger.LogDebug(
                    $"End retrieving departmentIds for filtering on sub-departments. {parentDepartmentIds.Count} departmentIds has been retrieved.");

                if (parentDepartmentIds.IsNullOrEmpty())
                {
                    return null;
                }
            }

            var customerIds = new List<int> { workContext.CurrentCustomerId };

            var userAccessChecking = await _userAccessService.CheckReadUserAccessAsync(workContext: workContext,
                ownerId: workContext.CurrentOwnerId,
                customerIds: customerIds,
                userExtIds: null,
                loginServiceClaims: null,
                userIds: null,
                userGroupIds: null,
                parentDepartmentIds: parentDepartmentIds,
                multiUserGroupFilters: null,
                userTypeIdsFilter: userTypeIds,
                userTypeExtIdsFilter: null,
                multipleUserTypeIdsFilter: null,
                multipleUserTypeExtIdsFilter: null);

            if (!userAccessChecking.IsAllowedAccess)
            {
                return null;
            }

            var userIds = userAccessChecking.UserIds;
            parentDepartmentIds = userAccessChecking.ParentDepartmentIds;
            var multiUserGroupFilters = userAccessChecking.MultiUserGroupFilters;
            var multiUserTypefilters = userAccessChecking.MultiUserTypeFilters;

            var query = UserQueryBuilder.InitQueryBuilder(_appSettingOptions, _userCryptoService,
                    _userRepository.GetQueryAsNoTracking(userEntityStatuses.ToArray()))
                .FilterByUserIds(userIds)
                .FilterByDepartmentIds(parentDepartmentIds, false, false)
                .FilterByMultiUserTypeFilters(multiUserTypefilters)
                .FilterByMultiUserGroupFilters(multiUserGroupFilters, null, null, null, null, null)
                .FilterByDate(createdAfter: createdAfter, createdBefore: createdBefore,
                    lastUpdatedAfter: lastUpdatedAfter, lastUpdatedBefore: lastUpdatedBefore,
                    expirationDateAfter: expirationDateAfter, expirationDateBefore: expirationDateBefore,
                    entityActiveDateAfter: entityActiveDateAfter, entityActiveDateBefore: entityActiveDateBefore)
                .Build();
            query = FilterOnDynamicAttributeDateTime(query, firstLoginAfter, firstLoginBefore, lastLoginAfter,
                lastLoginBefore,
                onboardingAfter, onboardingBefore);
            return query;
        }

        private PrivilegedUserAccountInfo MapToPrivilegedUserAccountInfo(PrivilegedUserAccountEntity item,
          List<CatalogItemDto> designationCatalog, List<(int UserTypeId, string Name)> systemRoleInfos, List<CatalogItemDto> orgUnitTypes, Dictionary<int, string> hierarchyPathNamesGroupByDepartment)
        {
            var isExternallyMasteredUser = item.Locked == 1;

            hierarchyPathNamesGroupByDepartment.TryGetValue(item.DepartmentId, out var departmentPathName);

            return new PrivilegedUserAccountInfo
            {
                UserId = item.UserId,
                ExtId = item.ExtId,
                DepartmentId = item.DepartmentId,
                DepartmentPathName= departmentPathName,
                FullName = $"{item.FirstName} {item.LastName}".Trim(),
                LastLoginDate = !string.IsNullOrEmpty(item.LastLoginDate)
                    ? DateTime.Parse(item.LastLoginDate)
                    : (DateTime?)null,
                Designation = isExternallyMasteredUser
                    ? item.JobTitle
                    : FindCatalogItemDto(designationCatalog, item.DesignationId)?.DisplayText,
                EmailAddress = item.EmailAddress,
                SystemRoles = GetUserTypeNames(item.UtuEntities, systemRoleInfos),
                TypeOfOrganization = FindCatalogItemDto(orgUnitTypes, item.TypeOfOrganizationUnitId)?.DisplayText,
                AccountTypeDisplayText = isExternallyMasteredUser
                    ? _appSettings.ExternallyMasteredUserReportDisplayText
                    : _appSettings.NonExternallyMasteredUserReportDisplayText,
                AccountType = isExternallyMasteredUser ?
                    AccountType.ExternalMastered :
                    AccountType.NonExternalMastered,
                Created = item.Created,
            };
        }
        private CatalogItemDto FindCatalogItemDto(List<CatalogItemDto> catalogItemDtos, string id)
        {
            return catalogItemDtos.FirstOrDefault(c =>
                string.Equals(c.Id.ToString(), id, StringComparison.CurrentCultureIgnoreCase));
        }
        public List<(int UserTypeId, string Name)> ExtractUserTypeInfo(int languageId,
            List<UserTypeEntity> userTypeEntities, ArchetypeEnum archetype)
        {
            var userTypeInfos = userTypeEntities.Where(ut => ut.ArchetypeId == (int) archetype)
                .Select(ut => (ut.UserTypeId, ut.LT_UserType.FirstOrDefault(a => a.LanguageId == languageId)?.Name))
                .OrderBy(a => a.Name)
                .ToList();
            return userTypeInfos;
        }

        private List<string> GetUserTypeNames(ICollection<UTUEntity> utuEntities,
            List<(int UserTypeId, string Name)> userTypeInfos)
        {
            return userTypeInfos.Where(ut => utuEntities.Any(utu => utu.UserTypeId == ut.UserTypeId))
                .Select(ut => ut.Name).ToList();
        }

        private static IQueryable<UserEntity> FilterOnDynamicAttributeDateTime(IQueryable<UserEntity> query, DateTime? firstLoginAfter, DateTime? firstLoginBefore, DateTime? lastLoginAfter,
            DateTime? lastLoginBefore, DateTime? onboardingAfter, DateTime? onboardingBefore)
        {
          
            var firstLoginPath = $"$.{UserJsonDynamicAttributeName.FirstLoginDate}";
            var lastLoginPath = $"$.{UserJsonDynamicAttributeName.LastLoginDate}";
            var onboardingDatePath = $"$.{UserJsonDynamicAttributeName.FinishedOnboardingDate}";

            if (firstLoginBefore.HasValue)
            {
                var firstLoginBeforeString = firstLoginBefore.Value.ConvertToJsonValueDateTimeString();
                query = query.Where(x =>
                    EfJsonExtensions.JsonValue(x.DynamicAttributes, firstLoginPath).CompareTo(firstLoginBeforeString) <= 0);
            }

            if (firstLoginAfter.HasValue)
            {
                var firstLoginAfterString = firstLoginAfter.Value.ConvertToJsonValueDateTimeString();
                query = query.Where(x =>
                    EfJsonExtensions.JsonValue(x.DynamicAttributes, firstLoginPath).CompareTo(firstLoginAfterString) >= 0);
            }

            if (lastLoginBefore.HasValue)
            {
                var lastLoginBeforeString = lastLoginBefore.Value.ConvertToJsonValueDateTimeString();
                query = query.Where(x =>
                    EfJsonExtensions.JsonValue(x.DynamicAttributes, lastLoginPath).CompareTo(lastLoginBeforeString) <= 0);
            }

            if (lastLoginAfter.HasValue)
            {
                var lastLoginAfterString = lastLoginAfter.Value.ConvertToJsonValueDateTimeString();
                query = query.Where(x =>
                    EfJsonExtensions.JsonValue(x.DynamicAttributes, lastLoginPath).CompareTo(lastLoginAfterString) >= 0);
            }

            if (onboardingBefore.HasValue)
            {
                var onboardingBeforeString = onboardingBefore.Value.ConvertToJsonValueDateTimeString();
                query = query.Where(x =>
                    EfJsonExtensions.JsonValue(x.DynamicAttributes, onboardingDatePath).CompareTo(onboardingBeforeString) >= 0);
            }

            if (onboardingAfter.HasValue)
            {
                var onboardingAfterString = onboardingAfter.Value.ConvertToJsonValueDateTimeString();
                query = query.Where(x =>
                    EfJsonExtensions.JsonValue(x.DynamicAttributes, onboardingDatePath).CompareTo(onboardingAfterString) >= 0);
            }

            return query;
        }


        private static int CountMemberInApprovalGroups(UserGenericDto userGenericDto, GrouptypeEnum groupType, List<KeyValuePair<int, int>> countAllMemberByUserGroup)
        {
            var primaryApprovalGroupIds = userGenericDto.OwnGroups
                .Where(g => g.Identity.Archetype == ArchetypeEnum.ApprovalGroup &&
                            g.Type == groupType)
                .Select(g => g.Identity.Id.Value)
                .ToList();
          return countAllMemberByUserGroup.Where(g => primaryApprovalGroupIds.Contains(g.Key)).Sum(g => g.Value);
        }

        private ApprovingOfficerInfo MapToApprovingOfficerInfo(UserGenericDto userGenericDto,  int primaryApprovalMemberCount, int alternativeApprovalMemberCount)
        {
            return new ApprovingOfficerInfo()
            {
                Identity = userGenericDto.Identity,
                EntityStatus = userGenericDto.EntityStatus,
                SSN = userGenericDto.SSN,
                DepartmentId = userGenericDto.DepartmentId,
                EmailAddress = userGenericDto.EmailAddress,
                CustomData = userGenericDto.CustomData,
                JsonDynamicAttributes = userGenericDto.JsonDynamicAttributes,
                FirstName = userGenericDto.FirstName,
                LastName = userGenericDto.LastName,
                DynamicAttributes = userGenericDto.DynamicAttributes,
                LoginServiceClaims = userGenericDto.LoginServiceClaims,
               // OwnGroups = userGenericDto.OwnGroups,
                Groups = userGenericDto.Groups,
                OtpValue = userGenericDto.OtpValue,
                DepartmentName = userGenericDto.DepartmentName,
                Created = userGenericDto.Created,
                DateOfBirth = userGenericDto.DateOfBirth,
                DepartmentAddress = userGenericDto.DepartmentAddress,
                ForceLoginAgain = userGenericDto.ForceLoginAgain,
                Gender = userGenericDto.Gender,
                IdpLocked = userGenericDto.IdpLocked,
                MobileCountryCode = userGenericDto.MobileCountryCode,
                MobileNumber = userGenericDto.MobileNumber,
                Password = userGenericDto.Password,
                ResetOtp = userGenericDto.ResetOtp,
                Roles = userGenericDto.Roles,
                Tag = userGenericDto.Tag,
                PrimaryApprovalMemberCount = primaryApprovalMemberCount,
                AlternativeApprovalMemberCount = alternativeApprovalMemberCount,
            };
        }

        private async Task<Dictionary<AccountType, int>> CountUserNotStartedOnBoardingUser(IAdvancedWorkContext workContext, DateTime? fromDate, DateTime? toDate)
        {
            var toDateJsonFilterString = (toDate ?? DateTime.MaxValue).ConvertToISO8601();

            //Count user has not started  yet. Or user started onboarding after toDate
            var jsonDynamicFiler = new List<string>
            {
                $"{UserJsonDynamicAttributeName.StartedOnboardingDate}>null,{toDateJsonFilterString}"
            };

            var externalMasteredUserCount =  await CountUserAsync(workContext, fromDate, toDate, jsonDynamicFiler, true);

            var nonExternalMasteredUserCount = await CountUserAsync(workContext, fromDate, toDate, jsonDynamicFiler, false);

            return new Dictionary<AccountType, int>
            {
                {AccountType.All, nonExternalMasteredUserCount + externalMasteredUserCount},
                {AccountType.ExternalMastered, externalMasteredUserCount},
                {AccountType.NonExternalMastered, nonExternalMasteredUserCount}
            };
        }

        private async Task<Dictionary<AccountType, int>> CountUserStartedOnBoardingUser(IAdvancedWorkContext workContext, DateTime? fromDate, DateTime? toDate)
        {
            var fromDateJsonFilterString = (fromDate ?? DateTime.MinValue).ConvertToISO8601();
            var toDateJsonFilterString = (toDate ?? DateTime.MaxValue).ConvertToISO8601();

            //Count User has started in given timespan but not completed in that time yet
            var jsonDynamicFiler = new List<string>
            {
                $"{UserJsonDynamicAttributeName.StartedOnboardingDate}>={fromDateJsonFilterString}",
                $"{UserJsonDynamicAttributeName.StartedOnboardingDate}<={toDateJsonFilterString}",
                $"{UserJsonDynamicAttributeName.FinishedOnboardingDate}>null,{toDateJsonFilterString}"
            };

            var externalMasteredUserCount = await CountUserAsync(workContext, null, null, jsonDynamicFiler, true);

            var nonExternalMasteredUserCount = await CountUserAsync(workContext, null, null, jsonDynamicFiler, false);

            return new Dictionary<AccountType, int>
            {
                {AccountType.All, nonExternalMasteredUserCount + externalMasteredUserCount},
                {AccountType.ExternalMastered, externalMasteredUserCount},
                {AccountType.NonExternalMastered, nonExternalMasteredUserCount}
            };

        }

        private async Task<Dictionary<AccountType, int>> CountUserFinishedOnBoardingUser(IAdvancedWorkContext workContext, DateTime? fromDate, DateTime? toDate)
        {
            var fromDateJsonFilterString = (fromDate ?? DateTime.MinValue).ConvertToISO8601();
            var toDateJsonFilterString = (toDate ?? DateTime.MaxValue).ConvertToISO8601();

            //Count User has completed in given timespan
            var jsonDynamicFiler = new List<string>
            {
                $"{UserJsonDynamicAttributeName.FinishedOnboardingDate}>={fromDateJsonFilterString}",
                $"{UserJsonDynamicAttributeName.FinishedOnboardingDate}<={toDateJsonFilterString}"
            };

            var externalMasteredUserCount = await CountUserAsync(workContext, null, null, jsonDynamicFiler, true);

            var nonExternalMasteredUserCount = await CountUserAsync(workContext, null, null, jsonDynamicFiler, false);

            return new Dictionary<AccountType, int>
            {
                {AccountType.All, nonExternalMasteredUserCount + externalMasteredUserCount},
                {AccountType.ExternalMastered, externalMasteredUserCount},
                {AccountType.NonExternalMastered, nonExternalMasteredUserCount}
            };
        }

        private async Task<int> CountUserAsync(IAdvancedWorkContext wokContext, DateTime? fromDate, DateTime? toDate, List<string> jsonDynamicFiler,
            bool externallyMastered)
        { 
            return  await _userService.CountUsersAsync(wokContext.CurrentOwnerId,
                customerIds: new List<int> { wokContext.CurrentCustomerId},
                statusIds: new List<EntityStatusEnum> {EntityStatusEnum.All},
                jsonDynamicData: jsonDynamicFiler,
                createdAfter: fromDate,
                createdBefore: toDate,
                externallyMastered: externallyMastered);
        }

        private async Task<UserAccountStatisticsDto> CalculateAccountStatisticsAsync(IAdvancedWorkContext workContext, List<EntityStatusEnum> entityStatuses, DateTime? fromDate,
            DateTime? toDate)
        {
    
            var userStatisticsInfos = await _userRepository.GetUserStatisticsInfos(workContext.CurrentOwnerId,
                workContext.CurrentCustomerId, entityStatuses: entityStatuses,  createdAfter: fromDate, createdBefore: toDate);

            var externallyMasteredUserStatisticsInfos = userStatisticsInfos.Where(u => u.Locked == 1).ToList();
            var nonExternallyMasteredUserStatisticsInfos =
                userStatisticsInfos.Except(externallyMasteredUserStatisticsInfos).ToList();


            var orgUnitTypes = await _learningCatalogClientService.GetOrganizationUnitTypes(workContext.CorrelationId);

            var orgUnitTypeNamesGroupById = userStatisticsInfos.Select(u => u.TypeOfOrganizationUnitId)
                .Distinct(StringComparer.CurrentCultureIgnoreCase)
                .Select(orgUnitTypeId => new
                {
                    orgUnitTypeId,
                    orgUnitTypeName = FindCatalogItemDto(orgUnitTypes, orgUnitTypeId)?.DisplayText
                })
                .OrderBy(a => a.orgUnitTypeName)
                .ToDictionary(a => a.orgUnitTypeId, a => a.orgUnitTypeName);

            var accountStatistics = new UserAccountStatisticsDto();

            var countByExternalMastered = externallyMasteredUserStatisticsInfos.Sum(a => a.NumberOfUser);
            var countByNonExternalMastered = nonExternallyMasteredUserStatisticsInfos.Sum(a => a.NumberOfUser);
            const string allOrganizationType = "All types of organisation";
            accountStatistics.Add((EntityStatusEnum.All, allOrganizationType), new Dictionary<AccountType, int>
            {
                {
                    AccountType.All, countByExternalMastered + countByNonExternalMastered
                },
                {AccountType.ExternalMastered, countByExternalMastered},
                {AccountType.NonExternalMastered, countByNonExternalMastered}
            });

            if (entityStatuses.IsNullOrEmpty())
            {
                entityStatuses = new List<EntityStatusEnum>();
                var values = Enum.GetValues(typeof(EntityStatusEnum));
                foreach (EntityStatusEnum entityStatusEnum in values)
                {
                    entityStatuses.Add(entityStatusEnum);
                }
            }

            foreach (EntityStatusEnum entityStatusEnum in entityStatuses)
            {
                if (entityStatusEnum == EntityStatusEnum.All) continue;

                var externallyMasteredUserStatisticsInfosGroupByStatus =
                    externallyMasteredUserStatisticsInfos.Where(a => a.EntityStatusId == (int) entityStatusEnum)
                        .ToList();

                var nonExternallyMasteredUserStatisticsInfosGroupByStatus =
                    nonExternallyMasteredUserStatisticsInfos.Where(a => a.EntityStatusId == (int) entityStatusEnum)
                        .ToList();

                var countStatusByExternalMastered =
                    externallyMasteredUserStatisticsInfosGroupByStatus.Sum(a => a.NumberOfUser);

                var countStatusByNonExternalMastered =
                    nonExternallyMasteredUserStatisticsInfosGroupByStatus.Sum(a => a.NumberOfUser);

                accountStatistics.Add((entityStatusEnum, allOrganizationType), new Dictionary<AccountType, int>
                {
                    {AccountType.All, countStatusByExternalMastered + countStatusByNonExternalMastered},
                    {AccountType.ExternalMastered, countStatusByExternalMastered},
                    {AccountType.NonExternalMastered, countStatusByNonExternalMastered}
                });

                if (externallyMasteredUserStatisticsInfosGroupByStatus.Count == 0 &&
                    nonExternallyMasteredUserStatisticsInfosGroupByStatus.Count == 0)
                {
                    continue;

                }

                foreach (var oganizationUnitType in orgUnitTypeNamesGroupById)
                {
                    var oganizationUnitTypeId = oganizationUnitType.Key;
                    var countOrgUnitByExternalMastered = externallyMasteredUserStatisticsInfosGroupByStatus
                        .Where(a => string.Equals(a.TypeOfOrganizationUnitId,
                            oganizationUnitTypeId, StringComparison.CurrentCultureIgnoreCase))
                        .Sum(a => a.NumberOfUser);

                    var countOrgUnitByNonExternalMastered = nonExternallyMasteredUserStatisticsInfosGroupByStatus
                        .Where(a => string.Equals(a.TypeOfOrganizationUnitId,
                            oganizationUnitTypeId, StringComparison.CurrentCultureIgnoreCase))
                        .Sum(a => a.NumberOfUser);

                    if (countOrgUnitByExternalMastered == 0 && countOrgUnitByNonExternalMastered == 0)
                        continue;

                    var oganizationUnitTypeName = string.IsNullOrEmpty(oganizationUnitType.Value)
                        ? "Unknown type of organisation"
                        : oganizationUnitType.Value;


                    if (accountStatistics.TryGetValue((entityStatusEnum, oganizationUnitTypeName),
                        out var existCountByStatus))
                    {
                        existCountByStatus[AccountType.All] =
                            existCountByStatus[AccountType.All] + countOrgUnitByExternalMastered +
                            countOrgUnitByNonExternalMastered;

                        existCountByStatus[AccountType.ExternalMastered] =
                            existCountByStatus[AccountType.ExternalMastered] + countOrgUnitByExternalMastered;

                        existCountByStatus[AccountType.NonExternalMastered] =
                            existCountByStatus[AccountType.NonExternalMastered] + countOrgUnitByNonExternalMastered;

                    }
                    else
                    {
                        var countByStatus = new Dictionary<AccountType, int>
                        {
                            {AccountType.All, countOrgUnitByExternalMastered + countOrgUnitByNonExternalMastered},
                            {AccountType.ExternalMastered, countOrgUnitByExternalMastered},
                            {AccountType.NonExternalMastered, countOrgUnitByNonExternalMastered}
                        };

                        accountStatistics.Add((entityStatusEnum, oganizationUnitTypeName), countByStatus);
                    }

                }

            }

            return accountStatistics;
        }

        private UserEventLogInfo GenerateEventLogInfo(GenericLogEventMessage logEventMessage, UserGenericDto user,
            string eventCreatedByUserId)
        {
            var eventInfo = new UserEventLogInfo()
            {
                EventId = logEventMessage.Id,
                Type = GetEventType(logEventMessage),
                Level = GetEventLogLevel(logEventMessage),
                CreatedByUser = user,
                Created = logEventMessage.Created,
                RoutingAction = logEventMessage.Routing.Action,
                SourceIp = logEventMessage.Payload.Identity?.SourceIp
            };
            eventInfo.EventInfo = ExtractEventInfo(eventInfo.Type, logEventMessage, eventCreatedByUserId, user);
            return eventInfo;
        }

        private dynamic ExtractEventInfo(UserEventType userEventType, GenericLogEventMessage logEventMessage, string eventCreatedByUserId,
            UserGenericDto user)
        {
            var messageBody = logEventMessage.Payload?.Body;
            if (messageBody == null) return null;
            var orginalEventInfo =(OrginalEventInfo)JObject.FromObject(messageBody).ToObject<OrginalEventInfo>();
            if (orginalEventInfo != null)
            {
                string message = orginalEventInfo.Message;
                if (!string.IsNullOrEmpty(message))
                {
                    if (!string.IsNullOrEmpty(eventCreatedByUserId) && user?.Identity.Id > 0)
                    {
                        return
                            message.Replace($"{eventCreatedByUserId} ", "");
                        //When user has found we remove user id from message to more meaning full
                    }
                    return message;
                }
                else if (userEventType == UserEventType.LoginFail)
                {
                    //For event type LoginFail, when user input invalid login infor, cxid log to system an object as event info
                    //we transform it to human readable message
                    if (!string.IsNullOrWhiteSpace(orginalEventInfo.Email))
                    {
                        if (user != null && string.IsNullOrEmpty(user.EmailAddress))
                        {
                            user.EmailAddress = orginalEventInfo.Email;
                        }
                        return $"User login failed due to invalid login info.";
                    }
                }
                else if (EventTypeMessageInfoMapping.TryGetValue(userEventType, out var eventInfo))
                {
                    if(eventInfo.Contains('{'))//Has parameter
                    {
                        eventInfo = eventInfo
                            .Replace("{Email}", orginalEventInfo.Email)
                            .Replace("{Phone}", orginalEventInfo.PhoneNumber);
                    }
                    return eventInfo;
                }        
            }
            return messageBody;

        }

        private UserEventType GetEventType(GenericLogEventMessage logEventMessage)
        {
            var eventTypeMapping =
                EventTypeRoutingActionMapping.FirstOrDefault(v => v.Value == logEventMessage.Routing.Action);
            return default(KeyValuePair<UserEventType, string>).Equals(eventTypeMapping)
                ? UserEventType.Unknown
                : eventTypeMapping.Key;
        }

        private EventLogLevel GetEventLogLevel(GenericLogEventMessage logEventMessage)
        {
            var actionRoutes = logEventMessage.Routing.Action.Split('.');
            if (actionRoutes.Contains("system_warn"))
                return EventLogLevel.Warn;
            if (actionRoutes.Contains("system_success"))
                return EventLogLevel.Info;
            //TODO: find ERROR level
            return EventLogLevel.Info;
        }

        private UserGenericDto FindUser(List<UserGenericDto> userDtos, string eventCreatedByUserId, bool defaultIfNull)
        {

            var existingUser = FindUser(userDtos, eventCreatedByUserId);
            if (existingUser == null && defaultIfNull)
            {
                existingUser = new UserGenericDto
                {
                    Identity = new IdentityDto
                    {
                        ExtId = eventCreatedByUserId
                    }
                };
            }

            return existingUser;

        }

        private UserGenericDto FindUser(List<UserGenericDto> userDtos, string loginServiceClaim)
        {
            return userDtos.FirstOrDefault(c => c.LoginServiceClaims.Any(l => l.Value == loginServiceClaim));
        }

        private async Task<List<UserGenericDto>> GetUserGenericDtosAsync(int ownerId, List<int> customerIds,
            List<string> loginServiceClaims, bool getDepartment, bool getRole,
            DateTime? userCreatedAfter = null,
            DateTime? userCreatedBefore = null)
        {
            var userEntities = new List<UserGenericDto>();
            var hasMoreData = true;
            int pageIndex = 1;
            do
            {
                var paginatedUserEntities = await _userService.GetUsersAsync<UserGenericDto>(ownerId: ownerId,
                    customerIds: customerIds, extIds: loginServiceClaims,
                    pageIndex: pageIndex, includeDepartment: getDepartment,
                    getLoginServiceClaims: true,
                    getRoles: getRole,
                    statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All },
                    createdAfter: userCreatedAfter,
                    createdBefore: userCreatedBefore);
                hasMoreData = paginatedUserEntities.HasMoreData;
                pageIndex++;
                if (paginatedUserEntities.Items.Count > 0)
                {
                    userEntities.AddRange(paginatedUserEntities.Items);
                }

            } while (hasMoreData);

            return userEntities;
        }
        private class OrginalEventInfo
        {
            public string Message { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string UserName { get; set; }
            public string ClientId { get; set; }
        }
    }
}
