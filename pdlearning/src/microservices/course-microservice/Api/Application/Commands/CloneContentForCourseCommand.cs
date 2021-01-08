using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class CloneContentForCourseCommand : BaseThunderCommand
    {
        public Guid FromCourseId { get; set; }

        public Guid ToCourseId { get; set; }
    }
}
