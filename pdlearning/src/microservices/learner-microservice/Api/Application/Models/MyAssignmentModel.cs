using System;
using Microservice.Learner.Domain.Entities;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Models
{
    public class MyAssignmentModel
    {
        public MyAssignmentModel(MyAssignment entity)
        {
            ParticipantAssignmentTrackId = entity.Id;
            RegistrationId = entity.RegistrationId;
            UserId = entity.UserId;
            AssignmentId = entity.AssignmentId;
            Status = entity.Status;
            SubmittedDate = entity.SubmittedDate;
            StartDate = entity.StartDate;
            EndDate = entity.EndDate;
        }

        public Guid ParticipantAssignmentTrackId { get; set; }

        public Guid RegistrationId { get; set; }

        public Guid UserId { get; set; }

        public Guid AssignmentId { get; set; }

        public MyAssignmentStatus Status { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
