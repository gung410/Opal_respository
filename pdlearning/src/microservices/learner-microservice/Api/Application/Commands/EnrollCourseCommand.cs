using System;
using System.Collections.Generic;
using Thunder.Platform.Cqrs;

namespace Microservice.Learner.Application.Commands
{
    public class EnrollCourseCommand : BaseThunderCommand
    {
        public Guid Id { get; } = Guid.NewGuid();

        public Guid CourseId { get; set; }

        public IEnumerable<Guid> LectureIds { get; set; }
    }
}
