using System;

namespace Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads
{
    public class LearnerActivityQuizPayload
    {
        public Guid PlayingSessionId { get; set; }

        public Guid FormId { get; set; }

        public Guid? FormAnswerId { get; set; }
    }
}
