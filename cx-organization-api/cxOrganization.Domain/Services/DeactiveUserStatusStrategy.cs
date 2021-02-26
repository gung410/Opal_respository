using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cxOrganization.Domain.BaseEnums;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.HttpClients;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.Services
{
    public class DeActiveUserStatusStrategy : UserStatusManagement
    {
        public const string Policy = "DeactivatePolicy";
        /// <inheritdoc />
        public DeActiveUserStatusStrategy(ILogger logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
        }

        /// <inheritdoc />
        protected override async Task<List<UserIdentityDto>> GetIdmUsersWillUpdateStatus(ChangeUserStatusSettings changeUserStatusSettings, IIdentityServerClientService identityServerClientService)
        {
            var users = new List<UserIdentityDto>();
            if (changeUserStatusSettings.TryGetValue(Policy, out var deactivatePolicy) && deactivatePolicy != null)
            {
                var expiredDate = DateTime.UtcNow.AddHours(-deactivatePolicy.LimitAbsenceHours);
                users = await identityServerClientService.GetUsersAsync(new UserFilterParams { LastLoginDateBefore = expiredDate, Status = (int)IdmUserStatus.Suspended });

                return users;
            }
            return users;
        }

        protected override async Task<List<UserGenericDto>> GetOrgUsersWillUpdateStatus(List<string> extIds, IUserService userService)
        {
            var users = userService.GetUsers<UserGenericDto>(statusIds: new List<EntityStatusEnum> {  EntityStatusEnum.Inactive },
                                                                     ownerId: 3001, customerIds: new List<int> { 2052 },
                                                                     extIds: extIds,
                                                                     getLoginServiceClaims: true)?.Items;
            return users;
        }

        protected override EntityStatusEnum GetDestinationStatus()
        {
            return EntityStatusEnum.Deactive;
        }

        protected override string GetJobName()
        {
            return nameof(DeActiveUserStatusStrategy);
        }
    }
}
