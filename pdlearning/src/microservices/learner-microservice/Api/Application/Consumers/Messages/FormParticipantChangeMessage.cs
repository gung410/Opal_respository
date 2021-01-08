using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Consumers
{
    public class FormParticipantChangeMessage
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid FormOriginalObjectId { get; set; }

        public Guid FormId { get; set; }

        public bool? IsStarted { get; set; }

        public FormParticipantStatus Status { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }
    }
}
