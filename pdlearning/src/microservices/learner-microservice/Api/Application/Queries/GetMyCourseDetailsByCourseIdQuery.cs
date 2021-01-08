using System;
using Microservice.Learner.Application.Models;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Queries
{
    public class GetMyCourseDetailsByCourseIdQuery : BaseThunderQuery<CourseModel>
    {
        public Guid CourseId { get; set; }
    }
}
