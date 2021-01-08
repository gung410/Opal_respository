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
    [OpalConsumer("csl.events.membership.added")]
    public class CommunityMembershipAddedInvitationConsumer : InboxSupportConsumer<CommunityMembershipAcceptedInvitationMessage>
    {
        private readonly IWebinarMeetingApplicationService _webinarMeetingApplicationService;
        private readonly IRepository<CommunityMembership> _communityMemberRepository;
        private readonly IThunderCqrs _thunderCqrs;
        private readonly ILogger<CommunityMembershipAddedInvitationConsumer> _logger;

        public CommunityMembershipAddedInvitationConsumer(
            IWebinarMeetingApplicationService webinarMeetingApplicationService,
            IThunderCqrs thunderCqrs,
            IRepository<CommunityMembership> communityMemberRepository,
            ILogger<CommunityMembershipAddedInvitationConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _webinarMeetingApplicationService = webinarMeetingApplicationService;
            _communityMemberRepository = communityMemberRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(CommunityMembershipAcceptedInvitationMessage message)
        {
            var existedCommunity = await _communityMemberRepository
                .FirstOrDefaultAsync(x => x.CommunityId == message.Community.Id && x.UserId == message.User.Guid);
            if (existedCommunity != null)
            {
                _logger.LogWarning(
                    "[CommunityMembershipAddedInvitationConsumer] Member with id {UserId} was existed in community {CommunityId}",
                    message.User.Guid,
                    message.Community.Id);
                return;
            }

            var createEventCommand = new CreateCommunityMembershipCommand
            {
                CommunityId = message.Community.Id,
                UserId = message.User.Guid,
                Role = message.Role
            };

            await _thunderCqrs.SendCommand(createEventCommand);
            await _webinarMeetingApplicationService.UpdateWebinarMeetingsByCommunityId(message.Community.Id);
        }
    }
}
