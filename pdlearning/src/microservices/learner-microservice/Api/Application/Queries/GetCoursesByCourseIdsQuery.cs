using System;
using System.Collections.Generic;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetCoursesByCourseIdsQuery : BaseThunderQuery<List<CourseModel>>
    {
        public IEnumerable<Guid> CourseIds { get; set; }
    }
}
