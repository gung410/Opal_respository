using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class CreateAssessmentAnswerCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public Guid AssessmentId { get; set; }

        public Guid ParticipantAssignmentTrackId { get; set; }

        public Guid UserId { get; set; }
    }
}
