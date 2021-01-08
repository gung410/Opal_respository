using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.course.participantassignmenttrack.updated")]
    public class ParticipantAssignmentTrackUpdatedConsumer : ScopedOpalMessageConsumer<ParticipantAssignmentTrackChangeMessage>
    {
        private readonly IRepository<MyAssignment> _myAssignmentRepository;
        private readonly IWriteMyOutstandingTaskLogic _myOutstandingTaskCudLogic;
        private readonly ILogger<ParticipantAssignmentTrackUpdatedConsumer> _logger;

        public ParticipantAssignmentTrackUpdatedConsumer(
            IRepository<MyAssignment> myAssignmentRepository,
            IWriteMyOutstandingTaskLogic myOutstandingTaskCudLogic,
            ILogger<ParticipantAssignmentTrackUpdatedConsumer> logger)
        {
            _logger = logger;
            _myAssignmentRepository = myAssignmentRepository;
            _myOutstandingTaskCudLogic = myOutstandingTaskCudLogic;
        }

        public async Task InternalHandleAsync(ParticipantAssignmentTrackChangeMessage message)
        {
            var existingMyAssignment = await _myAssignmentRepository
                .GetAll()
                .FirstOrDefaultAsync(_ => _.Id == message.Id);

            if (existingMyAssignment == null)
            {
                _logger.LogError(message: "MyAssignment not found with ParticipantAssignmentTrackId: {Id}", message.Id);
                return;
            }

            /*
             * Note: There are many cases that happen updating ParticipantAssignmentTrack.
             * For now only care for updating submitted date that notify finishing an assignment
             */
            existingMyAssignment.SubmittedDate = message.SubmittedDate;

            // Only Update status for MyAssignment if message.Status != MyAssignmentStatus.IncompletePendingSubmission
            existingMyAssignment.ChangedBy = message.UserId;
            existingMyAssignment.ChangedDate = message.ChangedDate;

            existingMyAssignment.Status = message.Status == MyAssignmentStatus.IncompletePendingSubmission
                ? existingMyAssignment.Status
                : message.Status;

            await _myAssignmentRepository.UpdateAsync(existingMyAssignment);

            if (!existingMyAssignment.NotCompleted())
            {
                await _myOutstandingTaskCudLogic.DeleteAssignmentTask(existingMyAssignment);
            }
        }
    }
}
