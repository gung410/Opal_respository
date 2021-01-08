using System;

namespace Microservice.Calendar.Application.Consumers.Messages.Models
{
    public class SessionModel
    {
        public Guid Id { get; set; }

        public Guid ClassRunId { get; set; }

        public string SessionTitle { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }
    }
}
