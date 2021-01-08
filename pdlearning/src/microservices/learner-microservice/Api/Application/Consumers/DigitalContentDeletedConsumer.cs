using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.content.deleted")]
    public class DigitalContentDeletedConsumer : ScopedOpalMessageConsumer<DigitalContentChangeMessage>
    {
        private readonly IRepository<DigitalContent> _digitalContentRepository;

        public DigitalContentDeletedConsumer(IRepository<DigitalContent> digitalContentRepository)
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

            await _digitalContentRepository.DeleteAsync(existingDigitalContent);
        }
    }
}
