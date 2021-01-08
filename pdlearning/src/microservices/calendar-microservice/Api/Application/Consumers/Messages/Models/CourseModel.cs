using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Consumers.Messages.Models
{
    public class CourseModel
    {
        public Guid Id { get; set; }

        public string CourseName { get; set; }

        public CourseStatus Status { get; set; }
    }
}
