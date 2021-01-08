using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.Commands
{
    public class AssignAssignmentCommand : BaseThunderCommand
    {
        public List<AssignAssignmentCommandRegistration> Registrations { get; set; }

        public Guid AssignmentId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class AssignAssignmentCommandRegistration
    {
        public Guid RegistrationId { get; set; }

        public Guid UserId { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
