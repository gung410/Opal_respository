using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class DeleteCourseCommand : BaseThunderCommand
    {
        public Guid CourseId { get; set; }
    }
}
