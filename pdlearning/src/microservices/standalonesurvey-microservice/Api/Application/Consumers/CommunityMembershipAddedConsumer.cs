using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.StandaloneSurvey.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.StandaloneSurvey.Application.Consumers
{
    [OpalConsumer("csl.events.membership.added")]
    public class CommunityMembershipAddedConsumer : OpalMessageConsumer<CslUserCommunityChangeMessage>
    {
        private readonly ILogger<CommunityMembershipAddedConsumer> _logger;
        private readonly IRepository<SyncedCslCommunityMember> _communityMembersRepo;

        public CommunityMembershipAddedConsumer(
            ILogger<CommunityMembershipAddedConsumer> logger,
            IRepository<SyncedCslCommunityMember> communityMembersRepo)
        {
            _logger = logger;
            _communityMembersRepo = communityMembersRepo;
        }

        protected override async Task InternalHandleAsync(CslUserCommunityChangeMessage message)
        {
            var isExistedMember = await _communityMembersRepo
                .GetAll()
                .Where(_ => _.UserId == message.User.UserId && _.CommunityId == message.Community.Id)
                .AnyAsync();

            if (isExistedMember)
            {
                _logger.LogError("Adds member but the member has existed. UserId = {userId} | CommunityId = {communityId} ", message.User.UserId, message.Community.Id);
                return;
            }

            var member = new SyncedCslCommunityMember(
                message.Community.Id,
                message.User.UserId,
                message.User.Email,
                message.User.Status,
                message.User.Visibility,
                message.User.DisplayName,
                message.User.Url,
                message.Role,
                message.User.CreatedAt,
                message.User.UpdatedAt);

            await _communityMembersRepo.InsertAsync(member);
        }
    }
}
