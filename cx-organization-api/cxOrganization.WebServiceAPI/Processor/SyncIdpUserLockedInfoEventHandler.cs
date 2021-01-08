using System;
using cxOrganization.Domain.Dtos.Users;
using cxOrganization.WebServiceAPI.Processor.Event;
using cxPlatform.Client.ConexusBase;
using Microsoft.Extensions.Logging;

namespace cxOrganization.WebServiceAPI.Processor
{
    public class SyncIdpUserLockedInfoEventHandler : SyncIdpUserInfoEventHandlerBase
    {
        public static readonly string AcceptedAction = "cxid.system_warn.locked.user";
        public SyncIdpUserLockedInfoEventHandler(ILogger<SyncIdpUserLockedInfoEventHandler> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        public override string Action => AcceptedAction;
        protected override void SetUpdatingValueForUser(UserGenericDto userDb, IdpEvent eventData)
        {
            userDb.AddOrUpdateJsonProperty(UserJsonDynamicAttributeName.IdpLock, true);
            userDb.EntityStatus.StatusId = EntityStatusEnum.IdentityServerLocked;
        }
    }
}