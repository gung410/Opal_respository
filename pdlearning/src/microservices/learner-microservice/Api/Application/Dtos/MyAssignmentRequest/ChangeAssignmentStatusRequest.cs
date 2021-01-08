using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Dtos
{
    public class ChangeAssignmentStatusRequest
    {
        public Guid RegistrationId { get; set; }

        public Guid AssignmentId { get; set; }

        public MyAssignmentStatus Status { get; set; }
    }
}
