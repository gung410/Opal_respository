using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Consumers
{
    [OpalConsumer("csl.events.membership.added")]
    public class CommunityMembershipChangedRoleConsumer : OpalMessageConsumer<CslUserCommunityChangeMessage>
    {
        private readonly ILogger<CommunityMembershipAddedConsumer> _logger;
        private readonly IRepository<SyncedCslCommunityMember> _communityMembersRepo;

        public CommunityMembershipChangedRoleConsumer(
            ILogger<CommunityMembershipAddedConsumer> logger,
            IRepository<SyncedCslCommunityMember> communityMembersRepo)
        {
            _logger = logger;
            _communityMembersRepo = communityMembersRepo;
        }

        protected override async Task InternalHandleAsync(CslUserCommunityChangeMessage message)
        {
            var existedMember = await _communityMembersRepo
                .GetAll()
                .Where(_ => _.UserId == message.User.UserId && _.CommunityId == message.Community.Id)
                .FirstOrDefaultAsync();

            if (existedMember == null)
            {
                _logger.LogError("Change member's role but can't found the member. UserId = {userId} | CommunityId = {communityId} ", message.User.UserId, message.Community.Id);
                return;
            }

            existedMember.Role = message.Role;

            await _communityMembersRepo.UpdateAsync(existedMember);
        }
    }
}
