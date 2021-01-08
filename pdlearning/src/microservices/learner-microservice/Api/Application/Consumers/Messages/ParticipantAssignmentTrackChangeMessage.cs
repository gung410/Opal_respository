using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Consumers
{
    public class ParticipantAssignmentTrackChangeMessage
    {
        public Guid Id { get; set; }

        public Guid RegistrationId { get; set; }

        public Guid UserId { get; set; }

        public Guid AssignmentId { get; set; }

        public MyAssignmentStatus Status { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
