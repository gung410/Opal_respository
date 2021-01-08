using System;

namespace Microservice.Course.Application.Models
{
    public class UpcomingSessionModel
    {
        public Guid ClassRunId { get; set; }

        public DateTime? StartDateTime { get; set; }
    }
}
