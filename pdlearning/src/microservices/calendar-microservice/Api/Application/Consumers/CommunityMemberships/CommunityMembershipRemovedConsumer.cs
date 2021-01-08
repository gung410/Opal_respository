using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Application.Services;
using Microservice.Calendar.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers.CommunityMemberships
{
    [OpalConsumer("csl.events.membership.removed")]
    public class CommunityMembershipRemovedConsumer : InboxSupportConsumer<CommunityMembershipRemovedMessage>
    {
        private readonly IWebinarMeetingApplicationService _webinarMeetingApplicationService;
        private readonly IRepository<CommunityMembership> _communityMembershipRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly ILogger<CommunityMembershipRemovedConsumer> _logger;

        public CommunityMembershipRemovedConsumer(
            IWebinarMeetingApplicationService webinarMeetingApplicationService,
            IThunderCqrs thunderCqrs,
            IRepository<CommunityMembership> communityMembershipRepository,
            ILogger<CommunityMembershipRemovedConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _webinarMeetingApplicationService = webinarMeetingApplicationService;
            _communityMembershipRepository = communityMembershipRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(CommunityMembershipRemovedMessage message)
        {
            var existedMembership = await _communityMembershipRepository
                .FirstOrDefaultAsync(x => x.CommunityId == message.Community.Id && x.UserId == message.User.Guid);
            if (existedMembership == null)
            {
                _logger.LogWarning(
                    "[CommunityMembershipRemovedConsumer] Membership with Id {MembershipId} of the Community with Id {CommunityId} was not existed.",
                    message.User.Guid,
                    message.Community.Id);
                return;
            }

            var deleteEventCommand = new DeleteCommunityMembershipCommand
            {
                CommunityId = message.Community.Id,
                UserId = message.User.Guid
            };

            await _thunderCqrs.SendCommand(deleteEventCommand);
            await _webinarMeetingApplicationService.UpdateWebinarMeetingsByCommunityId(message.Community.Id);
        }
    }
}
