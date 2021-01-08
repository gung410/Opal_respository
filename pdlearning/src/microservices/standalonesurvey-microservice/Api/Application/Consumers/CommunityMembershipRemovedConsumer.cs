using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Consumers
{
    [OpalConsumer("csl.events.membership.removed")]
    public class CommunityMembershipRemovedConsumer : OpalMessageConsumer<CslUserCommunityChangeMessage>
    {
        private readonly ILogger<CommunityMembershipRemovedConsumer> _logger;
        private readonly IRepository<SyncedCslCommunityMember> _communityMembersRepo;

        public CommunityMembershipRemovedConsumer(
            ILogger<CommunityMembershipRemovedConsumer> logger,
            IRepository<SyncedCslCommunityMember> communityMembersRepo)
        {
            _logger = logger;
            _communityMembersRepo = communityMembersRepo;
        }

        protected override async Task InternalHandleAsync(CslUserCommunityChangeMessage message)
        {
            var isExistingMember = await _communityMembersRepo
                .GetAll()
                .Where(_ => _.UserId == message.User.UserId && _.CommunityId == message.Community.Id)
                .AnyAsync();

            if (isExistingMember)
            {
                _logger.LogError("Remove membership can't found member. UserId = {userId} | CommunityId = {communityId} ", message.User.UserId, message.Community.Id);
                return;
            }

            await _communityMembersRepo.DeleteAsync(_ =>
                _.UserId == message.User.UserId && _.CommunityId == message.Community.Id);
        }
    }
}
