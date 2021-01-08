using System;

namespace Microservice.Calendar.Application.Consumers.Messages.Models
{
    public class CourseAssignmentModel
    {
        public Guid Id { get; set; }

        public Guid CourseId { get; set; }

        public Guid? ClassRunId { get; set; }

        public string Title { get; set; }
    }
}
