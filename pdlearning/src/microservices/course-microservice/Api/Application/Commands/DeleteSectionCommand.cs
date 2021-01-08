using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class DeleteSectionCommand : BaseThunderCommand
    {
        public Guid SectionId { get; set; }

        public Guid CourseId { get; set; }
    }
}
