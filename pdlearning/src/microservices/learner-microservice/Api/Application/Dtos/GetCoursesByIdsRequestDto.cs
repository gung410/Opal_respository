using System;
using System.Collections.Generic;

namespace Microservice.Learner.Application.Dtos
{
    public class GetCoursesByIdsRequestDto
    {
        public IEnumerable<Guid> CourseIds { get; set; }
    }
}
