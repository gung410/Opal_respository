using System;
using Microservice.Calendar.Domain.Enums;

namespace Microservice.Calendar.Application.Consumers.Messages.Models
{
    public class ClassRunModel
    {
        public Guid Id { get; set; }

        public string ClassTitle { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public ClassRunStatus Status { get; set; }
    }
}
