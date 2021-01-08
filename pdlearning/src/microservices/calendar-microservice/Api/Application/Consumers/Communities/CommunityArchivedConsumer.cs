using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Conexus.Opal.InboxPattern;
using Microservice.Calendar.Application.Commands;
using Microservice.Calendar.Application.Consumers.Messages;
using Microservice.Calendar.Domain.Entities;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Cqrs;

namespace Microservice.Calendar.Application.Consumers.Communities
{
    [OpalConsumer("csl.events.community.archived")]
    [OpalConsumer("csl.events.subcommunity.archived")]
    public class CommunityArchivedConsumer : InboxSupportConsumer<CommunityArchivedMessage>
    {
        private readonly IThunderCqrs _thunderCqrs;
        private readonly IRepository<Community> _communityRepo;

        public CommunityArchivedConsumer(
            IThunderCqrs thunderCqrs,
            IRepository<Community> communityRepo)
        {
            _thunderCqrs = thunderCqrs;
            _communityRepo = communityRepo;
        }

        public async Task InternalHandleAsync(CommunityArchivedMessage message)
        {
            var community = await _communityRepo.FirstOrDefaultAsync(c => c.Id == message.Id);
            if (community == null)
            {
                var createCommunityCommand = new CreateCommunityCommand
                {
                    Id = message.Id,
                    Title = message.Name,
                    Status = message.Status,
                    ParentId = message.MainCommunityId,
                    OwnerId = message.CreatedBy
                };

                await _thunderCqrs.SendCommand(createCommunityCommand);
            }
            else
            {
                var command = new ArchiveCommunityCommand
                {
                    Id = message.Id,
                    Title = message.Name,
                    Status = message.Status,
                    OwnerId = message.CreatedBy,
                    ParentId = message.MainCommunityId
                };

                await _thunderCqrs.SendCommand(command);
            }
        }
    }
}
