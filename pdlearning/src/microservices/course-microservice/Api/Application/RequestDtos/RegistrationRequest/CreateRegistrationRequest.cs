using System;
using System.Collections.Generic;
using System.Linq;
using Microservice.Course.Application.Commands;
using Microservice.Course.Domain.Enums;

namespace Microservice.Course.Application.RequestDtos
{
    public class CreateRegistrationRequest
    {
        public Guid ApprovingOfficer { get; set; }

        public Guid? AlternativeApprovingOfficer { get; set; }

        public RegistrationType RegistrationType { get; set; }

        public IEnumerable<RegistrationDto> Registrations { get; set; }

        public SaveRegistrationCommand ToCommand()
        {
            return new SaveRegistrationCommand()
            {
                ApprovingOfficer = ApprovingOfficer,
                AlternativeApprovingOfficer = AlternativeApprovingOfficer,
                RegistrationType = RegistrationType,
                Registrations = Registrations?
                .Select(p => new SaveRegistrationCommandRegistration()
                {
                    Id = Guid.NewGuid(),
                    CourseId = p.CourseId,
                    ClassRunId = p.ClassRunId
                }).ToList() ??
                new List<SaveRegistrationCommandRegistration>(),
            };
        }
    }
}
