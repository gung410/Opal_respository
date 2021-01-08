using System;

namespace Microservice.Analytics.Application.Consumers.Learner.Messages
{
    public class BaseLearnerActivityMessage<TPayload> where TPayload : class
    {
        public string EventName { get; set; }

        public TPayload Payload { get; set; }

        public DateTime Time { get; set; }

        public Guid SessionId { get; set; }

        public Guid UserId { get; set; }

        public string SourceIp { get; set; }
    }
}
