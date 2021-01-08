using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.content.created")]
    [OpalConsumer("microservice.events.content.cloned")]
    public class DigitalContentCreatedConsumer : ScopedOpalMessageConsumer<DigitalContentChangeMessage>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;

        public DigitalContentCreatedConsumer(IRepository<DigitalContent> digitalContentRepository)
        {
            _digitalContentRepository = digitalContentRepository;
        }

        public async Task InternalHandleAsync(DigitalContentChangeMessage message)
        {
            var existedDigitalContent = await _digitalContentRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .AnyAsync();

            if (existedDigitalContent)
            {
                return;
            }

            var digitalContent = new DigitalContent
            {
                Id = message.Id,
                Status = message.Status,
                Title = message.Title,
                CreatedDate = message.CreatedDate,
                OriginalObjectId = message.OriginalObjectId,
                ChangedDate = message.ChangedDate,
                Description = message.Description,
                OwnerId = message.OwnerId,
                Type = message.Type,
                FileExtension = message.FileExtension
            };

            await _digitalContentRepository.InsertAsync(digitalContent);
        }
    }
}
