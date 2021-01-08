using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class CloneCourseCommand : BaseThunderCommand
    {
        public Guid Id { get; set; }

        public Guid NewId { get; set; }

        public bool FromCoursePlanning { get; set; }
    }
}
