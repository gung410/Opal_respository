using System;
using System.Collections.Generic;
using System.Linq;
using Thunder.Platform.Cqrs;

#pragma warning disable SA1402 // File may only contain a single type
namespace Microservice.Course.Application.Commands
{
    public class NominateUserCommand : BaseThunderCommand
    {
        public IEnumerable<NominateUserCommandRegistration> Registrations { get; set; }

        public List<Guid> GetRegistrationCourseIds()
        {
            return Registrations.Select(p => p.CourseId).Distinct().ToList();
        }

        public List<Guid> GetRegistrationClassrunIds()
        {
            return Registrations.Select(p => p.ClassRunId).Distinct().ToList();
        }
    }

    public class NominateUserCommandRegistration
    {
        public Guid ApprovingOfficer { get; set; }

        public Guid? AlternativeApprovingOfficer { get; set; }

        public Guid UserId { get; set; }

        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }
    }
}
#pragma warning restore SA1402 // File may only contain a single type
