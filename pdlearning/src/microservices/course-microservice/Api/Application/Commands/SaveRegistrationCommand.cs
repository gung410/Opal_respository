using System;
using System.Collections.Generic;
using Microservice.Course.Domain.Enums;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.Commands
{
    public class SaveRegistrationCommand : BaseThunderCommand
    {
        public Guid ApprovingOfficer { get; set; }

        public Guid? AlternativeApprovingOfficer { get; set; }

        public RegistrationType RegistrationType { get; set; }

        public List<SaveRegistrationCommandRegistration> Registrations { get; set; }
    }

    public class SaveRegistrationCommandRegistration
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
