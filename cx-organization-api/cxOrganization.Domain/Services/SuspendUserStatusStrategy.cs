using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.Domain.HttpClients;
using cxOrganization.Domain.Settings;
using cxPlatform.Client.ConexusBase;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace cxOrganization.Domain.Services
{
    public class SuspendUserStatusStrategy : UserStatusManagement
    {
        public const string Policy = "SuspendPolicy";
        /// <inheritdoc />
        public SuspendUserStatusStrategy(ILogger logger, IServiceProvider serviceProvider) : base(logger, serviceProvider)
        {
        }

        /// <inheritdoc />
        protected override async Task<List<UserIdentityDto>> GetIdmUsersWillUpdateStatus(ChangeUserStatusSettings changeUserStatusSetting, IIdentityServerClientService identityServerClientService)
        {
            var users = new List<UserIdentityDto>();
            if (changeUserStatusSetting.TryGetValue(Policy, out var suspendPolicy) && suspendPolicy != null)
            {
                var expiredDate = DateTime.UtcNow.AddHours(-suspendPolicy.LimitAbsenceHours);
                users = await identityServerClientService.GetUsersAsync(new UserFilterParams { LastLoginDateBefore = expiredDate});

                return users;
            }
            return users;
        }

        protected override async Task<List<UserGenericDto>> GetOrgUsersWillUpdateStatus(List<string> extIds, IUserService userService)
        {
            var users =  await userService.GetUsersAsync<UserGenericDto>(statusIds: new List<EntityStatusEnum> { EntityStatusEnum.Active },
                                                                     ownerId: 3001, customerIds: new List<int> { 2052 },
                                                                     extIds: extIds,
                                                                     getLoginServiceClaims: true);
            return users?.Items;
        }

        protected override EntityStatusEnum GetDestinationStatus()
        {
            return EntityStatusEnum.Inactive;
        }
    }
}
