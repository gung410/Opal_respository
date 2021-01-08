using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class UserNominationRequest
    {
        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }

        public List<NominatedRegistrationDto> NominatedRegistrations { get; set; } = new List<NominatedRegistrationDto>();
    }
}
