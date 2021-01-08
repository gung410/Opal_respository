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
    [OpalConsumer("csl.events.community.deleted")]
    [OpalConsumer("csl.events.subcommunity.deleted")]
    public class CommunityDeletedConsumer : InboxSupportConsumer<CommunityDeletedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<Community> _communityRepository;
        private readonly ILogger<CommunityDeletedConsumer> _logger;

        public CommunityDeletedConsumer(IThunderCqrs thunderCqrs, IRepository<Community> communityRepository, ILogger<CommunityDeletedConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _communityRepository = communityRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(CommunityDeletedMessage message)
        {
            var existedCommunity = await _communityRepository.FirstOrDefaultAsync(x => x.Id == message.Id);
            if (existedCommunity == null)
            {
                _logger.LogWarning("[CommunityDeletedConsumer] Community with Id {CommunityId} was not existed.", message.Id);
                return;
            }

            var deleteCommunityCommand = new DeleteCommunityCommand
            {
                CommunityId = existedCommunity.Id
            };
            await _thunderCqrs.SendCommand(deleteCommunityCommand);
        }
    }
}
