using System;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class CourseDeletedMessage
    {
        public Guid Id { get; set; }
    }
}
