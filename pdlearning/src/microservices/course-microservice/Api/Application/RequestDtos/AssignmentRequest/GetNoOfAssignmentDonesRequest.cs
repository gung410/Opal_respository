using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetNoOfAssignmentDonesRequest
    {
        public Guid ClassRunId { get; set; }

        public IEnumerable<Guid> RegistrationIds { get; set; }
    }
}
