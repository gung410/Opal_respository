using System;
using Thunder.Platform.Cqrs;

namespace Microservice.Course.Application.Commands
{
    public class TransferCourseOwnershipCommand : BaseThunderCommand
    {
        public Guid CourseId { get; set; }

        public Guid NewOwnerId { get; set; }
    }
}
