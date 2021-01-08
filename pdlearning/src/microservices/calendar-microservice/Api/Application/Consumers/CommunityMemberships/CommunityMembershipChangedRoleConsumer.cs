using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Domain.Entities;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers.CommunityMemberships
{
    [OpalConsumer("csl.events.membership.changed_role")]
    public class CommunityMembershipChangedRoleConsumer : InboxSupportConsumer<CommunityMembershipChangedRoleMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<CommunityMembership> _communityMembershipRepository;
        private readonly ILogger<CommunityMembershipChangedRoleConsumer> _logger;

        public CommunityMembershipChangedRoleConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<CommunityMembership> communityMembershipRepository,
            ILogger<CommunityMembershipChangedRoleConsumer> logger)
        {
            _thunderCqrs = thunderCqrs;
            _communityMembershipRepository = communityMembershipRepository;
            _logger = logger;
        }

        public async Task InternalHandleAsync(CommunityMembershipChangedRoleMessage message)
        {
            var existedMembership = await _communityMembershipRepository
                .FirstOrDefaultAsync(x => x.CommunityId == message.Community.Id && x.UserId == message.User.Guid);
            if (existedMembership == null)
            {
                _logger.LogWarning(
                    "[CommunityMembershipChangedRoleConsumer] Membership with Id {MembershipId} of the Community with Id {CommunityId} was not existed.",
                    message.User.Guid,
                    message.Community.Id);
                return;
            }

            var updateEventCommand = new ChangeRoleCommunityMembershipCommand
            {
                CommunityId = message.Community.Id,
                UserId = message.User.Guid,
                Role = message.Role
            };
            await _thunderCqrs.SendCommand(updateEventCommand);
        }
    }
}
