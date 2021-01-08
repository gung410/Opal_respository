using System;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class CourseAssignmentDeletedMessage
    {
        /// <summary>
        /// Assignment id.
        /// </summary>
        public Guid Id { get; set; }
    }
}
