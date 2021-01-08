using System;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyCourseByCourseIdQuery : BaseThunderQuery<MyCourseModel>
    {
        public Guid CourseId { get; set; }
    }
}
