using System.Linq;
using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Learner.Application.BusinessLogic.Abstractions;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Thunder.Platform.Core.Domain.Repositories;

namespace Microservice.Learner.Application.Consumers
{
    [OpalConsumer("microservice.events.course.participantassignmenttrack.created")]
    public class ParticipantAssignmentTrackCreatedConsumer : ScopedOpalMessageConsumer<ParticipantAssignmentTrackChangeMessage>
    {
        private readonly IRepository<MyAssignment> _myAssignmentRepository;
        private readonly IWriteMyOutstandingTaskLogic _myOutstandingTaskCudLogic;

        public ParticipantAssignmentTrackCreatedConsumer(
            IRepository<MyAssignment> myAssignmentRepository,
            IWriteMyOutstandingTaskLogic myOutstandingTaskCudLogic)
        {
            _myAssignmentRepository = myAssignmentRepository;
            _myOutstandingTaskCudLogic = myOutstandingTaskCudLogic;
        }

        public async Task InternalHandleAsync(ParticipantAssignmentTrackChangeMessage message)
        {
            var anyExistingMyAssignment = await _myAssignmentRepository
                .GetAll()
                .Where(p => p.Id == message.Id)
                .AnyAsync();

            if (anyExistingMyAssignment)
            {
                return;
            }

            var myAssignment = new MyAssignment
            {
                Id = message.Id,
                AssignmentId = message.AssignmentId,
                RegistrationId = message.RegistrationId,
                Status = MyAssignmentStatus.NotStarted,
                UserId = message.UserId,
                CreatedBy = message.CreatedBy,
                StartDate = message.StartDate,
                EndDate = message.EndDate
            };

            await _myAssignmentRepository.InsertAsync(myAssignment);

            await _myOutstandingTaskCudLogic.InsertAssignmentTask(myAssignment);
        }
    }
}
