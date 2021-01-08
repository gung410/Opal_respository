using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.form.participant.updated")]
    public class FormParticipantUpdatedConsumer : ScopedOpalMessageConsumer<FormParticipantChangeMessage>
    {
        private readonly IWriteMyOutstandingTaskLogic _myOutstandingTaskCudLogic;
        private readonly IRepository<FormParticipant> _formParticipantRepository;

        public FormParticipantUpdatedConsumer(
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

            existingParticipant.Update(
                message.UserId,
                message.FormOriginalObjectId,
                message.FormId,
                message.Status,
                message.ChangedDate);

            await _formParticipantRepository.UpdateAsync(existingParticipant);

            // Remove task if the form participant status is Completed.
            if (existingParticipant.IsCompleted())
            {
                await _myOutstandingTaskCudLogic.DeleteStandaloneFormTask(existingParticipant);
            }
        }
    }
}
