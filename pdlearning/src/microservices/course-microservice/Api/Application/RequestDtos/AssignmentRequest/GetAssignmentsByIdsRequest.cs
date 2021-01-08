using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class GetAssignmentsByIdsRequest
    {
        public IEnumerable<Guid> Ids { get; set; }

        public bool IncludeQuizForm { get; set; }
    }
}
