using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class ExtractContentUrlCommand : BaseThunderCommand
    {
        public Guid? CourseId { get; set; }

        public Guid? ClassrunId { get; set; }

        public Guid? AssignmentId { get; set; }

        public Guid? LectureId { get; set; }
    }
}
