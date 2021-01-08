using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetAllLectureIdsBelongToCourseQuery : BaseThunderQuery<List<Guid>>
    {
        public Guid CourseId { get; set; }
    }
}
