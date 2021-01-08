using System.Threading.Tasks;
using Conexus.Opal.Connector.RabbitMQ.Core;
using Microservice.Course.Application.BusinessLogics.EntityCud;
using Microservice.Course.Domain.Entities;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Core.Domain.Repositories;
using Thunder.Platform.Core.Timing;

namespace Microservice.Course.Application.Consumers
{
    [OpalConsumer("microservice.events.learner.assignment.updated")]
    public class AssignmentUpdatedConsumer : ScopedOpalMessageConsumer<MyAssignmentChangeMessage>
    {
        public async Task InternalHandleAsync(
            MyAssignmentChangeMessage message,
            IReadOnlyRepository<ParticipantAssignmentTrack> readParticipantAssignmentTrackRepository,
            ParticipantAssignmentTrackCudLogic participantAssignmentTrackCudLogic)
        {
            if (message.Status == MyAssignmentMessageStatus.InProgress)
            {
                var participantAssignmentTrack = await readParticipantAssignmentTrackRepository.GetAsync(message.ParticipantAssignmentTrackId);

                if (participantAssignmentTrack.Status == ParticipantAssignmentTrackStatus.NotStarted)
                {
                    participantAssignmentTrack.Status = ParticipantAssignmentTrackStatus.InProgress;
                    participantAssignmentTrack.ChangedBy = message.ChangedBy.GetValueOrDefault();
                    participantAssignmentTrack.ChangedDate = Clock.Now;

                    await participantAssignmentTrackCudLogic.Update(participantAssignmentTrack);
                }
            }
        }
    }
}
