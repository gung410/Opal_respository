using System;
using System.Collections.Generic;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetListCoursesByListIdQuery : BaseThunderQuery<List<CourseModel>>
    {
        public List<Guid> ListIds { get; set; }
    }
}
