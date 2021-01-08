using System;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class CourseAssignmentChangedMessage
    {
        /// <summary>
        /// Assignment id.
        /// </summary>
        public Guid Id { get; set; }

        public string Title { get; set; }

        public Guid? ClassRunId { get; set; }

        public Guid? CourseId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
