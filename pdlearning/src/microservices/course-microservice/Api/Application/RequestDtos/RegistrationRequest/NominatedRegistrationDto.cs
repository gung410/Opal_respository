using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class NominatedRegistrationDto
    {
        public Guid ApprovingOfficer { get; set; }

        public Guid? AlternativeApprovingOfficer { get; set; }

        public Guid UserId { get; set; }
    }
}
