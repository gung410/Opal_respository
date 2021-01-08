using System;
using Microservice.Course.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Queries
{
    public class GetCourseByIdQuery : BaseThunderQuery<CourseModel>
    {
        public Guid Id { get; set; }
    }
}
