using cxOrganization.Domain.AdvancedWorkContext;
using cxOrganization.Domain.BaseEnums;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.HttpClients;
using cxOrganization.Domain.Settings;
using cxOrganization.Domain.Validators;
using cxPlatform.Client.ConexusBase;
using cxPlatform.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NPOI.HSSF.Record.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
namespace cxOrganization.Domain.Services
{
    public abstract class UserStatusManagement
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        protected UserStatusManagement(
            ILogger logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        protected abstract Task<List<UserIdentityDto>> GetIdmUsersWillUpdateStatus(ChangeUserStatusSettings changeUserStatusSetting, IIdentityServerClientService identityServerClientService);

        protected abstract Task<List<UserGenericDto>> GetOrgUsersWillUpdateStatus(List<string> extIds, IUserService identityServerClientService);

        protected abstract EntityStatusEnum GetDestinationStatus();

        protected abstract string GetJobName();

        public async Task UpdateUserStatus()
        {
            using (var insideBackgroundTaskScope = _serviceProvider.CreateScope())
            {
                var workContext = insideBackgroundTaskScope.ServiceProvider.GetService<IAdvancedWorkContext>();
                var userService = insideBackgroundTaskScope.ServiceProvider.GetService<Func<ArchetypeEnum, IUserService>>()(ArchetypeEnum.Unknown);
                var identityServerClientService = insideBackgroundTaskScope.ServiceProvider.GetService<IIdentityServerClientService>();
                var changeUserStatusSetting = insideBackgroundTaskScope.ServiceProvider.GetService<IOptions<ChangeUserStatusSettings>>()?.Value;

                var failUpdatedOrgUsers = new List<UserGenericDto>();

                var idmUsers = await GetIdmUsersWillUpdateStatus(changeUserStatusSetting, identityServerClientService);

                if (idmUsers == null || idmUsers.Count == 0)
                {
                    _logger.LogInformation($"NO USER FOUND IN IDM, END SCHEDULED JOB THAT CHANGES USER STATUS");
                    return;
                }

                var idmUserExtIds = idmUsers.Select(x => x.Id).ToList();
                var orgUsers = await GetOrgUsersWillUpdateStatus(idmUserExtIds, userService);
                if (orgUsers == null || !orgUsers.Any())
                {
                    _logger.LogInformation($"NO USER FOUND IN ORG, END BACKGROUND JOB THAT CHANGES USER STATUS");
                    return;
                }

                try
                {
                    _logger.LogWarning($"START TO CHANGE IDM USER TO {GetDestinationStatus().ToString()} STATUS FOR THE NUMBER OF {idmUsers.Count} USERS");
                    var successUpdatedStatusIdmUsers = await UpdateUserStatusInIdm(GetDestinationStatus(), identityServerClientService, workContext, idmUsers);
                    
                    _logger.LogWarning($"THE NUMBER OF {successUpdatedStatusIdmUsers.Count} IDM USERS STATUS ARE AUTOMATICALLY CHANGED TO {GetDestinationStatus().ToString()} BY {GetJobName()}");
                    var neededUpdatedOrgUsers = orgUsers.Where(u => successUpdatedStatusIdmUsers.Any(idmUser => idmUser.User.Id == u.Identity.ExtId)).ToList();
                   
                    _logger.LogWarning($"START TO CHANGE ORG USER TO {GetDestinationStatus().ToString()} STATUS FOR THE NUMBER OF {orgUsers.Count} USERS");
                    var successSuspendedOrgUsers = UpdateUserStatusInOrg(GetDestinationStatus(), userService, workContext, neededUpdatedOrgUsers);
                    _logger.LogWarning($"THE NUMBER OF {successSuspendedOrgUsers.Count} ORG USERS STATUS ARE AUTOMATICALLY CHANGED TO {GetDestinationStatus().ToString()} BY {GetJobName()}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"THERE IS AN ERROR WHEN CHANGING USER STATUS TO {GetDestinationStatus().ToString()}, END BACKGROUND JOB THAT CHANGES USER STATUS");
                }

            }
        }

        private List<ConexusBaseDto> UpdateUserStatusInOrg(
            EntityStatusEnum destinationStatus,
            IUserService userService,
            IAdvancedWorkContext workContext,
            List<UserGenericDto> orgUsers)
        {
            if (orgUsers == null)
            {
                return new List<ConexusBaseDto>();
            }

            var successChangingStatusUsers = new List<ConexusBaseDto>();
            foreach (var user in orgUsers)
            {
                try
                {
                    var validationSpecification = (new HierarchyDepartmentValidationBuilder())
                        .ValidateDepartment(user.DepartmentId, ArchetypeEnum.Unknown)
                        .SkipCheckingArchetype()
                        .WithStatus(EntityStatusEnum.All)
                        .IsDirectParent()
                        .Create();

                    workContext.IsEnableFiltercxToken = false;
                    workContext.CurrentOwnerId = 3001;
                    workContext.CurrentCustomerId = 2052;

                    user.EntityStatus.StatusId = destinationStatus;

                    if (destinationStatus == EntityStatusEnum.Inactive)
                    {
                        user.EntityStatus.StatusReasonId = EntityStatusReasonEnum.Inactive_Automatically_Inactivity;
                        user.AddJsonPropertyIfNotExisting($"Automatically_Inactive_By_{GetJobName()}_Datetime", DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
                    }

                    if (destinationStatus == EntityStatusEnum.Deactive)
                    {
                        user.EntityStatus.StatusReasonId = EntityStatusReasonEnum.Deactive_AutomaticallySetDeactive;
                        user.AddJsonPropertyIfNotExisting($"Automatically_Deactive_By_{GetJobName()}_Datetime", DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'"));
                    }

                    var updatedUser = userService.UpdateUser(validationSpecification, user);
                    successChangingStatusUsers.Add(updatedUser);
                    _logger.LogWarning($"SUCCESS TO UPDATE ORG USER {user.Identity.ExtId} TO {GetDestinationStatus().ToString()} STATUS BY {GetJobName()}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"THERE IS AN ERROR WHEN CHANGING ORG USER {user.Identity.ExtId} TO {GetDestinationStatus().ToString()} STATUS BY {GetJobName()}");
                }
            }

            return successChangingStatusUsers.Distinct().ToList();
        }

        private async Task<List<UserIdentityResponseDto>> UpdateUserStatusInIdm(
            EntityStatusEnum destinationStatus,
            IIdentityServerClientService identityServerClientService,
            IAdvancedWorkContext workContext,
            List<UserIdentityDto> idmUsers)
        {
            if (idmUsers == null)
                return new List<UserIdentityResponseDto>();

            var successChangingStatusUsers = new List<UserIdentityResponseDto>();

            foreach (var user in idmUsers)
            {
                try
                {
                    workContext.IsEnableFiltercxToken = false;
                    workContext.CurrentOwnerId = 3001;
                    workContext.CurrentCustomerId = 2052;
                    var updatedUsers = await identityServerClientService.UpdateUserStatusAsync(user.Id, MapIdmToOrgStatus(destinationStatus));
                    user.Status = (int)MapIdmToOrgStatus(destinationStatus);
                    successChangingStatusUsers.Add(updatedUsers);
                    _logger.LogWarning($"SUCCESS TO UPDATE IDM USER {user.Id} TO {GetDestinationStatus().ToString()} STATUS BY {GetJobName()}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"THERE IS AN ERROR WHEN CHANGING IDM USER {user.Id} TO {GetDestinationStatus().ToString()} STATUS BY {GetJobName()}");
                }
            }
            return successChangingStatusUsers.Distinct().ToList();
        }

        private IdmUserStatus MapIdmToOrgStatus(EntityStatusEnum statusId)
        {
            switch (statusId)
            {
                case EntityStatusEnum.Pending:
                    return IdmUserStatus.Suspended;
                case EntityStatusEnum.PendingApproval1st:
                    return IdmUserStatus.Suspended;
                case EntityStatusEnum.PendingApproval2nd:
                    return IdmUserStatus.Suspended;
                case EntityStatusEnum.PendingApproval3rd:
                    return IdmUserStatus.Suspended;
                case EntityStatusEnum.New:
                    return IdmUserStatus.Invite;
                case EntityStatusEnum.Deactive:
                    return IdmUserStatus.DeActivated;
                case EntityStatusEnum.Active:
                    return IdmUserStatus.Active;
                case EntityStatusEnum.Inactive:
                    return IdmUserStatus.Suspended;
                default:
                    return IdmUserStatus.Invite;
            }
        }
    }
}
