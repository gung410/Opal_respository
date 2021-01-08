using System;
using System.Collections.Generic;
using Microservice.Calendar.Application.Consumers.Messages.Models;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Consumers.Messages
{
    public class ClassRunChangedMessage
    {
        public Guid Id { get; set; }

        public string ClassTitle { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public ClassRunStatus Status { get; set; }

        public List<SessionModel> Sessions { get; set; }

        public CourseModel Course { get; set; }
    }
}
