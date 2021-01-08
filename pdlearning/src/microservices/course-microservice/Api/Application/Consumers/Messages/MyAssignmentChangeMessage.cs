using System;

namespace Microservice.Course.Application.Consumers
{
    public class MyAssignmentChangeMessage
    {
        public Guid ParticipantAssignmentTrackId { get; set; }

        public Guid RegistrationId { get; set; }

        public Guid UserId { get; set; }

        public Guid AssignmentId { get; set; }

        public MyAssignmentMessageStatus Status { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public bool IsDeleted { get; set; }
    }
}
