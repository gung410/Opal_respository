using System;
using System.Collections.Generic;

namespace Microservice.Learner.Application.Dtos
{
    public class EnrollCourseRequestDto
    {
        public Guid CourseId { get; set; }

        public IEnumerable<Guid> LectureIds { get; set; }
    }
}
