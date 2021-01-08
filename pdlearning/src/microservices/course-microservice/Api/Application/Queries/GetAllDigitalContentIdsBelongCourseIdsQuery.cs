using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetAllDigitalContentIdsBelongCourseIdsQuery : BaseThunderQuery<CourseIdMapListDigitalContentIdModel[]>
    {
        public Guid[] CourseIds { get; set; }
    }
}
