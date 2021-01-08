using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Consumers
{
    [OpalConsumer("csl.events.community.created")]
    [OpalConsumer("csl.events.subcommunity.created")]
    public class CommunityCreatedConsumer : OpalMessageConsumer<CslCommunityMessage>
    {
        private readonly ILogger<CommunityCreatedConsumer> _logger;
        private readonly IRepository<SyncedCslCommunity> _communitiesRepo;
        private readonly IRepository<SyncedCslCommunityMember> _communityMembersRepo;

        public CommunityCreatedConsumer(
            ILogger<CommunityCreatedConsumer> logger,
            IRepository<SyncedCslCommunity> communitiesRepo,
            IRepository<SyncedCslCommunityMember> communityMembersRepo)
        {
            _logger = logger;
            _communitiesRepo = communitiesRepo;
            _communityMembersRepo = communityMembersRepo;
        }

        protected override async Task InternalHandleAsync(CslCommunityMessage message)
        {
            var existedCommunity = await _communitiesRepo
                .GetAll()
                .Where(_ => _.Id == message.Id)
                .AnyAsync();

            if (existedCommunity)
            {
                _logger.LogError("Existed community with {communityId}", message.Id);
                return;
            }

            var community = new SyncedCslCommunity(
                message.Id,
                message.Name,
                message.MainCommunityId,
                message.Url,
                message.Visibility,
                message.JoinPolicy,
                message.Status,
                message.CreatedBy,
                message.UpdatedBy,
                message.CreatedAt,
                message.CreatedAt);

            await _communitiesRepo.InsertAsync(community);
        }
    }
}
