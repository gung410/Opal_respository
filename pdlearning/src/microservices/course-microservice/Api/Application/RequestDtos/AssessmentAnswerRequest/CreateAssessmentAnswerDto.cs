using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microservice.Course.Application.Commands;
using Microservice.Course.Domain.Entities;

namespace Microservice.Course.Application.RequestDtos
{
    public class CreateAssessmentAnswerDto
    {
        public Guid? Id { get; set; }

        public Guid AssessmentId { get; set; }

        public Guid ParticipantAssignmentTrackId { get; set; }

        public Guid UserId { get; set; }

        public CreateAssessmentAnswerCommand ToCommand()
        {
            return new CreateAssessmentAnswerCommand()
            {
                Id = Id ?? Guid.NewGuid(),
                AssessmentId = AssessmentId,
                ParticipantAssignmentTrackId = ParticipantAssignmentTrackId,
                UserId = UserId
            };
        }
    }
}
