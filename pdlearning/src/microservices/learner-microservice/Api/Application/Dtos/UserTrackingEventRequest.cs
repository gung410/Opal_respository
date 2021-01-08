using System;

namespace Microservice.Learner.Application.Dtos
{
    public class UserTrackingEventRequest
    {
        public string EventName { get; set; }

        public DateTime Time { get; set; }

        public Guid SessionId { get; set; }

        public string SourceIp { get; set; }

        public string UserTrackingEventAsJson { get; set; }
    }
}
