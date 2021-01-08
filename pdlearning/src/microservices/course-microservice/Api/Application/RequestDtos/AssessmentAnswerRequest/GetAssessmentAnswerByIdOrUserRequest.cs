using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetAssessmentAnswerByIdOrUserRequest
    {
        public Guid? Id { get; set; }

        public Guid? ParticipantAssignmentTrackId { get; set; }

        public Guid? UserId { get; set; }
    }
}
