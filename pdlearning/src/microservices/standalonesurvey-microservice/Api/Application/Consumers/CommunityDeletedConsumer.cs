using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Consumers
{
    [OpalConsumer("csl.events.community.deleted")]
    [OpalConsumer("csl.events.subcommunity.deleted")]
    public class CommunityDeletedConsumer : OpalMessageConsumer<CslCommunityMessage>
    {
        private readonly ILogger<CommunityDeletedConsumer> _logger;
        private readonly IRepository<SyncedCslCommunity> _communitiesRepo;

        public CommunityDeletedConsumer(
            ILogger<CommunityDeletedConsumer> logger,
            IRepository<SyncedCslCommunity> communitiesRepo)
        {
            _logger = logger;
            _communitiesRepo = communitiesRepo;
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

            await _communitiesRepo.DeleteAsync(_ => _.Id == message.Id);
        }
    }
}
