using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetAssessmentAnswerByIdOrUserQuery : BaseThunderQuery<AssessmentAnswerModel>
    {
        public Guid? Id { get; set; }

        public Guid? ParticipantAssignmentTrackId { get; set; }

        public Guid? UserId { get; set; }
    }
}
