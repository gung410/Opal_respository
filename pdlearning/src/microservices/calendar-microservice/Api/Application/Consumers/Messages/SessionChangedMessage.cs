using System;
using Microservice.Calendar.Application.Consumers.Messages.Models;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class SessionChangedMessage
    {
        public Guid Id { get; set; }

        public Guid ClassRunId { get; set; }

        public string SessionTitle { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public ClassRunModel ClassRun { get; set; }

        public CourseModel Course { get; set; }
    }
}
