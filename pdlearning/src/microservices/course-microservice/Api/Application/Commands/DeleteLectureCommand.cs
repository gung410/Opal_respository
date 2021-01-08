using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class DeleteLectureCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }
    }
}
