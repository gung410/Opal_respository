using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.content.updated")]
    [OpalConsumer("microservice.events.content.rollback")]
    public class DigitalContentUpdatedConsumer : ScopedOpalMessageConsumer<DigitalContentChangeMessage>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;

        public DigitalContentUpdatedConsumer(IRepository<DigitalContent> digitalContentRepository)
        {
            _digitalContentRepository = digitalContentRepository;
        }

        public async Task InternalHandleAsync(DigitalContentChangeMessage message)
        {
            var existingDigitalContent = await _digitalContentRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .FirstOrDefaultAsync();

            if (existingDigitalContent == null)
            {
                return;
            }

            existingDigitalContent.Status = message.Status;
            existingDigitalContent.Title = message.Title;
            existingDigitalContent.CreatedDate = message.CreatedDate;
            existingDigitalContent.OriginalObjectId = message.OriginalObjectId;
            existingDigitalContent.ChangedDate = message.ChangedDate;
            existingDigitalContent.Description = message.Description;
            existingDigitalContent.OwnerId = message.OwnerId;
            existingDigitalContent.Type = message.Type;
            existingDigitalContent.FileExtension = message.FileExtension;

            await _digitalContentRepository.UpdateAsync(existingDigitalContent);
        }
    }
}
