using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class ChangeAssignmentStatusCommand : BaseThunderCommand
    {
        public Guid RegistrationId { get; set; }

        public Guid AssignmentId { get; set; }

        public MyAssignmentStatus Status { get; set; }
    }
}
