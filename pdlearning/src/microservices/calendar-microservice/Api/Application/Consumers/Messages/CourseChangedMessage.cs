using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class CourseChangedMessage
    {
        public Guid Id { get; set; }

        public string CourseName { get; set; }

        public CourseStatus Status { get; set; }
    }
}
