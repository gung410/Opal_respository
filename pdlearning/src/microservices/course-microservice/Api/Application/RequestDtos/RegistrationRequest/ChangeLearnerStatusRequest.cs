using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class ChangeLearnerStatusRequest
    {
        public Guid ClassRunId { get; set; }

        public List<Guid> RegistrationIds { get; set; }

        public bool IsCompleted { get; set; }
    }
}
