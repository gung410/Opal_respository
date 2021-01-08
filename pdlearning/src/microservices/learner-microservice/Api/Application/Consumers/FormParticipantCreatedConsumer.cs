using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.form.participant.created")]
    public class FormParticipantCreatedConsumer : ScopedOpalMessageConsumer<FormParticipantChangeMessage>
    {
        private readonly IRepository<FormParticipant> _formParticipantRepository;

        public FormParticipantCreatedConsumer(IRepository<FormParticipant> formParticipantRepository)
        {
            _formParticipantRepository = formParticipantRepository;
        }

        public async Task InternalHandleAsync(FormParticipantChangeMessage message)
        {
            var anyExistingParticipant = await _formParticipantRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .AnyAsync();

            if (anyExistingParticipant)
            {
                return;
            }

            var formParticipant = new FormParticipant
            {
                Id = message.Id,
                Status = message.Status,
                CreatedDate = message.CreatedDate,
                ChangedDate = message.ChangedDate,
                FormOriginalObjectId = message.FormOriginalObjectId,
                FormId = message.FormId,
                UserId = message.UserId
            };

            await _formParticipantRepository.InsertAsync(formParticipant);
        }
    }
}
