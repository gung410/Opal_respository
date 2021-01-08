using System;
using System.Collections.Generic;

namespace Microservice.Course.Application.RequestDtos
{
    public class ValidateNominateLearnersRequest
    {
        public Guid CourseId { get; set; }

        public List<Guid> UserId { get; set; }
    }
}
