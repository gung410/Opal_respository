using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers.Communities
{
    [OpalConsumer("csl.events.community.created")]
    [OpalConsumer("csl.events.community.updated")]
    [OpalConsumer("csl.events.subcommunity.created")]
    [OpalConsumer("csl.events.subcommunity.updated")]
    public class CommunityChangedConsumer : InboxSupportConsumer<CommunityChangedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<Community> _communityRepository;
        private readonly ILogger<CommunityChangedConsumer> _logger;

        public CommunityChangedConsumer(IThunderCqrs thunderCqrs, IRepository<Community> communityRepository, ILogger<CommunityChangedConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _communityRepository = communityRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(CommunityChangedMessage message)
        {
            if (message.MainCommunityId.HasValue)
            {
                var mainCommunity = await _communityRepository.FirstOrDefaultAsync(x => x.Id == message.MainCommunityId.Value);
                if (mainCommunity == null)
                {
                    _logger.LogWarning("[CommunityChangedConsumer] Main Community with Id {MainCommunityId} does not exist.", message.MainCommunityId);
                    return;
                }
            }

            var existedCommunity = await _communityRepository.FirstOrDefaultAsync(x => x.Id == message.Id);
            if (existedCommunity == null)
            {
                var createCommunityCommand = new CreateCommunityCommand
                {
                    Id = message.Id,
                    Title = message.Name,
                    ParentId = message.MainCommunityId,
                    OwnerId = message.CreatedBy,
                    Status = message.Status
                };
                await _thunderCqrs.SendCommand(createCommunityCommand);
                return;
            }

            var updateCommunityCommand = new UpdateCommunityCommand
            {
                Id = existedCommunity.Id,
                Title = message.Name,
                ParentId = message.MainCommunityId,
                Status = message.Status
            };
            await _thunderCqrs.SendCommand(updateCommunityCommand);
        }
    }
}
