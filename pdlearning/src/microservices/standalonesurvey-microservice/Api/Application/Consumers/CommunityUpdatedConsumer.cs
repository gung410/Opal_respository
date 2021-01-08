using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Consumers
{
    [OpalConsumer("csl.events.community.updated")]
    [OpalConsumer("csl.events.community.archived")]
    [OpalConsumer("csl.events.community.reactive")]
    [OpalConsumer("csl.events.subcommunity.updated")]
    [OpalConsumer("csl.events.subcommunity.archived")]
    [OpalConsumer("csl.events.subcommunity.reactive")]
    public class CommunityUpdatedConsumer : OpalMessageConsumer<CslCommunityMessage>
    {
        private readonly ILogger<CommunityUpdatedConsumer> _logger;
        private readonly IRepository<SyncedCslCommunity> _communitiesRepo;

        public CommunityUpdatedConsumer(
            ILogger<CommunityUpdatedConsumer> logger,
            IRepository<SyncedCslCommunity> communitiesRepo)
        {
            _logger = logger;
            _communitiesRepo = communitiesRepo;
        }

        protected override async Task InternalHandleAsync(CslCommunityMessage message)
        {
            var existingCommunity = await _communitiesRepo
                .GetAll()
                .Where(_ => _.Id == message.Id)
                .FirstOrDefaultAsync();

            if (existingCommunity == null)
            {
                _logger.LogError("Update community can't found community with {communityId}", message.Id);
                return;
            }

            existingCommunity.Update(
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

            await _communitiesRepo
                .UpdateAsync(existingCommunity);
        }
    }
}
