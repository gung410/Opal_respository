using System;

namespace Microservice.Course.Application.RequestDtos
{
    public class TransferCourseOwnershipRequest
    {
        public Guid CourseId { get; set; }

        public Guid NewOwnerId { get; set; }
    }
}
