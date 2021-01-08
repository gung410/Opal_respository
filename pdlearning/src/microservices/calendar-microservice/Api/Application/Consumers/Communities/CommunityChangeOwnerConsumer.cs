using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Domain.Entities;
using Microservice.Calendar.Domain.Enums;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers.Communities
{
    [OpalConsumer("csl.events.community.change_owner")]
    [OpalConsumer("csl.events.subcommunity.change_owner")]
    public class CommunityChangeOwnerConsumer : InboxSupportConsumer<CommunityChangeOwnerMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<Community> _communityRepository;
        private readonly IRepository<CommunityMembership> _communityMembershipRepository;
        private readonly ILogger<CommunityChangeOwnerConsumer> _logger;

        public CommunityChangeOwnerConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<Community> communityRepository,
            IRepository<CommunityMembership> communityMembershipRepository,
            ILogger<CommunityChangeOwnerConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _communityRepository = communityRepository;
            _communityMembershipRepository = communityMembershipRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(CommunityChangeOwnerMessage message)
        {
            var existedCommunity = await _communityRepository.FirstOrDefaultAsync(x => x.Id == message.Id);
            var existedOwnerCommunity = await _communityMembershipRepository
               .FirstOrDefaultAsync(x => x.CommunityId == message.Id && x.Role == CommunityMembershipRole.Owner);
            if (existedCommunity == null || existedOwnerCommunity == null)
            {
                _logger.LogWarning("[CommunityChangeOwnerConsumer] Community with Id {CommunityId} was not existed.", message.Id);
                return;
            }

            var updateEventCommand = new ChangeOwnerCommunityCommand
            {
                CommunityId = message.Id,
                NewOwnerId = message.CreatedBy
            };
            await _thunderCqrs.SendCommand(updateEventCommand);
        }
    }
}
