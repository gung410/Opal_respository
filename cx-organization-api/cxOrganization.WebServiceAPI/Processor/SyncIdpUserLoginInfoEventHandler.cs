using System;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.WebServiceAPI.Processor.Event;
using cxPlatform.Client.ConexusBase;
using Microsoft.Extensions.Logging;

namespace cxOrganization.WebServiceAPI.Processor
{
    public class SyncIdpUserLoginInfoEventHandler : SyncIdpUserInfoEventHandlerBase
    {
        public static readonly string AcceptedAction = "cxid.system_success.login.user";
        public SyncIdpUserLoginInfoEventHandler(ILogger<SyncIdpUserLoginInfoEventHandler> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        public override string Action => AcceptedAction;
        protected override void SetUpdatingValueForUser(UserGenericDto userDb, IdpEvent eventData)
        {
            if (userDb.EntityStatus.StatusId == EntityStatusEnum.IdentityServerLocked || userDb.EntityStatus.StatusId == EntityStatusEnum.New)
            {
                userDb.EntityStatus.StatusId = EntityStatusEnum.Active;
            }

            if (userDb.GetJsonPropertyValue(UserJsonDynamicAttributeName.FirstLoginDate) == null)
            {
                userDb.AddOrUpdateJsonProperty(UserJsonDynamicAttributeName.FirstLoginDate, eventData.Created);
            }

            userDb.AddOrUpdateJsonProperty(UserJsonDynamicAttributeName.LastLoginDate, eventData.Created);
        }
    }
}