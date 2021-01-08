using cxOrganization.Domain;
using cxOrganization.Domain.ApiClient;
using cxOrganization.Domain.Business.Queries.ApprovingOfficer;
using cxOrganization.Domain.Common;
using cxOrganization.Domain.Dtos.DataHub;
using cxOrganization.Domain.Dtos.UserGroups;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.Services;
using cxOrganization.Domain.Settings;
using cxOrganization.WebServiceAPI.Models.AuditLog;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace cxOrganization.WebServiceAPI.Controllers
{
    [Authorize]
    public class AuditLogController : ApiControllerBase
    {
        private readonly ILogger<AuditLogController> _logger;
        private readonly IDataHubQueryApiClient _dataHubQueryApiClient;
        private readonly IWorkContext _workContext;
        private readonly IUserService _userService;
        private readonly IUserGroupService _userGroupService;
        private readonly SearchApprovingOfficersQueryHandler _searchApprovingOfficersQueryHandler;
        private readonly AppSettings _appSettings;

        public AuditLogController(
            ILogger<AuditLogController> logger,
            IDataHubQueryApiClient dataHubQueryApiClient,
            IWorkContext workContext,
            Func<ArchetypeEnum, IUserService> userService,
            IUserGroupService userGroupService,
            SearchApprovingOfficersQueryHandler searchApprovingOfficersQueryHandler,
            IOptions<AppSettings> appSettingsOption
            )
        {
            _logger = logger;
            _dataHubQueryApiClient = dataHubQueryApiClient;
            _workContext = workContext;
            _userService = userService(ArchetypeEnum.Unknown);
            _userGroupService = userGroupService;
            _searchApprovingOfficersQueryHandler = searchApprovingOfficersQueryHandler;
            _appSettings = appSettingsOption.Value;
        }

        /// <summary>
        /// Get the audit log of a user.
        /// </summary>
        /// <param name="auditLogParameter"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json", "text/csv")]
        [Route("auditlog")]
        [ProducesResponseType(typeof(PaginatedList<GenericLogEventMessage>), 200)]
        public async Task<IActionResult> Index([FromBody] AuditLogParameter auditLogParameter)
        {
            var userBeingRetrieved = DomainHelper
                .GetUserByExtId<UserGenericDto>(_userService, auditLogParameter.UserExtId, new List<EntityStatusEnum> { EntityStatusEnum.All });
            if (userBeingRetrieved == null)
            {
                _logger.LogWarning($"Could not find user with ExtId '{auditLogParameter.UserExtId}'");
                return NoContent();
            }

            var auditLogPaging = await _dataHubQueryApiClient.GetAuditLogsAsync(new RequestContext(_workContext), userBeingRetrieved, auditLogParameter.PageIndex, auditLogParameter.PageSize);
            var auditLogs = auditLogPaging.Items;

            var executorExtIds = auditLogs.Select(a => a?.Payload?.Identity?.UserId).Distinct().ToList();
            var executors = await GetExecutors(userBeingRetrieved, executorExtIds);

            await BuildApprovalGroupInfo(auditLogs);

            BuildExecutorInfo(auditLogs, executors);

            RemoveSensitiveData(auditLogs);

            return CreateResponse(auditLogPaging);
        }

        private void RemoveSensitiveData(List<GenericLogEventMessage> auditLogs)
        {
            var shouldHideSsn = ShouldHideSsn();
            var shouldHideDateOfBirth = ShouldHideDateOfBirth();
            foreach (var auditLogEntry in auditLogs)
            {
                // The Source IP Address is only needed for the event login failed many times, so we should remove for other event types.
                if (!string.Equals(auditLogEntry.Routing.Action, AuditLogEvents.LoginFailed, StringComparison.CurrentCultureIgnoreCase)
                    && auditLogEntry.Payload.Identity != null)
                {
                    auditLogEntry.Payload.Identity.SourceIp = null;
                }

                // Remove SSN & Date of Birth.
                if (IsUserCreatedOrUpdatedRoutingAction(auditLogEntry.Routing.Action)
                    && (shouldHideSsn || shouldHideDateOfBirth))
                {
                    if (auditLogEntry.Payload.Body == null) continue;
                    if (auditLogEntry.Payload.Body.userData == null) continue;

                    if (shouldHideSsn)
                    {
                        auditLogEntry.Payload.Body.userData.ssn = null;
                    }
                    if (shouldHideDateOfBirth)
                    {
                        auditLogEntry.Payload.Body.userData.dateOfBirth = null;
                    }
                }
            }
        }

        private bool IsUserCreatedOrUpdatedRoutingAction(string routingAction)
        {
            return string.Equals(routingAction, AuditLogEvents.UserCreated, StringComparison.CurrentCultureIgnoreCase)
                || string.Equals(routingAction, AuditLogEvents.UserUpdated, StringComparison.CurrentCultureIgnoreCase);
        }

        private void BuildExecutorInfo(List<GenericLogEventMessage> auditLogs, List<UserGenericDto> executors)
        {
            foreach (var auditLog in auditLogs)
            {
                var executor = executors.FirstOrDefault(p => p.Identity.ExtId == auditLog?.Payload?.Identity?.UserId);
                if (executor != null)
                {
                    auditLog.Executor = new ExecutorInfo
                    {
                        ExtId = executor.Identity.ExtId,
                        FullName = executor.GetFullName(),
                        AvatarUrl = GetAvatarUrl(executor)
                    };
                }
            }
        }

        private async Task<List<UserGenericDto>> GetExecutors(UserGenericDto userBeingRetrieved, List<string> executorExtIds)
        {
            var executors = executorExtIds.Count > 0
                            ? ( await _userService
                                .GetUsersAsync<UserGenericDto>(extIds: executorExtIds, statusIds: new List<EntityStatusEnum> { EntityStatusEnum.All }, ignoreCheckReadUserAccess: true))
                                .Items
                            : new List<UserGenericDto>();
            if (!executors.Any(u => u.Identity.Id == userBeingRetrieved.Identity.Id))
            {
                executors.Add(userBeingRetrieved);
            }

            return executors;
        }

        private string GetAvatarUrl(UserDtoBase user)
        {
            const string avatarUrlKey = "avatarUrl";
            if (user.JsonDynamicAttributes != null && user.JsonDynamicAttributes.ContainsKey(avatarUrlKey))
            {
                return user.JsonDynamicAttributes[avatarUrlKey];
            }
            return string.Empty;
        }

        private async Task BuildApprovalGroupInfo(List<GenericLogEventMessage> auditLogs)
        {
            var changeApprovalGroupMemberEvents = auditLogs
                            .Where(p => string.Equals(p.Routing.Action, AuditLogEvents.ApprovalGroupMemberAdded, StringComparison.CurrentCultureIgnoreCase)
                                || string.Equals(p.Routing.Action, AuditLogEvents.ApprovalGroupMemberRemoved, StringComparison.CurrentCultureIgnoreCase))
                            .ToList();

            InitApprovalGroupInfo(changeApprovalGroupMemberEvents);
            if (!changeApprovalGroupMemberEvents.Any(p => p.ApprovalGroupInfo != null))
            {
                return;
            }

            var approvalGroups = await GetApprovalGroups(changeApprovalGroupMemberEvents);

            BuildFinalApprovalGroupInfo(changeApprovalGroupMemberEvents, approvalGroups);
        }

        private static void InitApprovalGroupInfo(List<GenericLogEventMessage> changeApprovalGroupMemberEvents)
        {
            foreach (var eventObj in changeApprovalGroupMemberEvents)
            {
                var jbody = eventObj.Payload.Body as JObject;
                if (jbody == null) continue;
                var body = jbody.ToObject<ChangeApprovalGroupMemberDto>();
                if (body == null) continue;

                if (body.UserGroupId > 0)
                {
                    eventObj.ApprovalGroupInfo = new ApprovalGroupInfo
                    {
                        ApprovalGroupId = body.UserGroupId
                    };
                }
            }
        }

        private async Task<List<ApprovalGroupDto>> GetApprovalGroups(List<GenericLogEventMessage> changeApprovalGroupMemberEvents)
        {
            var approvalGroupIds = changeApprovalGroupMemberEvents
                            .Where(p => p.ApprovalGroupInfo != null)
                            .Select(p => p.ApprovalGroupInfo.ApprovalGroupId).ToList();
            var approvalGroups = (await _searchApprovingOfficersQueryHandler.HandleAsync(new SearchApprovingOfficersQuery
            {
                ApprovalGroupIds = approvalGroupIds
            })).Items.Where(p => p.ApproverId > 0).ToList();
            return approvalGroups;
        }

        private void BuildFinalApprovalGroupInfo(List<GenericLogEventMessage> changeApprovalGroupMemberEvents, List<ApprovalGroupDto> approvalGroups)
        {
            // Build Approval Group Info
            foreach (var eventObj in changeApprovalGroupMemberEvents)
            {
                if (eventObj.ApprovalGroupInfo != null)
                {
                    var approvalGroup = approvalGroups.FirstOrDefault(g => g.Identity.Id == eventObj.ApprovalGroupInfo.ApprovalGroupId);
                    if (approvalGroup != null)
                    {
                        eventObj.ApprovalGroupInfo.FullName = approvalGroup.Name;
                        eventObj.ApprovalGroupInfo.AvatarUrl = approvalGroup.AvatarUrl;
                        eventObj.ApprovalGroupInfo.Type = approvalGroup.Type;
                    }
                }
            }
        }

        protected bool ShouldHideSsn()
        {
            //We should only hide SSN when api is authenticate by user token to keep backward compatibility for system to system integration 
            return _appSettings.HideSSN && !string.IsNullOrEmpty(_workContext.Sub);
        }

        private bool ShouldHideDateOfBirth()
        {
            //We should only hide date of birth when api is authenticate by user token to keep backward compatibility for system to system integration 
            return _appSettings.HideDateOfBirth && !string.IsNullOrEmpty(_workContext.Sub);
        }
    }
}