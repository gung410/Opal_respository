using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetLearnerCourseViolationQuery : BaseThunderQuery<GetLearnerCourseViolationQueryResult>
    {
        public Guid CourseId { get; set; }

        public Guid ClassRunId { get; set; }
    }
}
