using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.form.participant.deleted")]
    public class FormParticipantDeletedConsumer : ScopedOpalMessageConsumer<FormParticipantChangeMessage>
    {
        private readonly IWriteMyOutstandingTaskLogic _myOutstandingTaskCudLogic;
        private readonly IRepository<FormParticipant> _formParticipantRepository;

        public FormParticipantDeletedConsumer(
            IWriteMyOutstandingTaskLogic myOutstandingTaskCudLogic,
            IRepository<FormParticipant> formParticipantRepository)
        {
            _myOutstandingTaskCudLogic = myOutstandingTaskCudLogic;
            _formParticipantRepository = formParticipantRepository;
        }

        public async Task InternalHandleAsync(FormParticipantChangeMessage message)
        {
            var existingParticipant = await _formParticipantRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .FirstOrDefaultAsync();

            if (existingParticipant == null)
            {
                return;
            }

            await _formParticipantRepository.DeleteAsync(existingParticipant);

            await _myOutstandingTaskCudLogic.DeleteStandaloneFormTask(existingParticipant);
        }
    }
}
