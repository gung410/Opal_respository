using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class OverrideRegistrationCourseCriteriaRequest
    {
        public List<Guid> RegistrationIds { get; set; }

        public Guid ClassrunId { get; set; }
    }
}
