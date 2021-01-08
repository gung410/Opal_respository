using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class DeleteLearningPathCourseByCourseIdCommand : BaseThunderCommand
    {
        public Guid CourseId { get; set; }
    }
}
