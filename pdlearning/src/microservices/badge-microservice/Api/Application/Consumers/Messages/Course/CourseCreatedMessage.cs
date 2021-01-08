using System;

namespace Microservice.Badge.Application.Consumers.Messages
{
    public class CourseCreatedMessage
    {
        public Guid Id { get; set; }

        public string PDActivityType { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
