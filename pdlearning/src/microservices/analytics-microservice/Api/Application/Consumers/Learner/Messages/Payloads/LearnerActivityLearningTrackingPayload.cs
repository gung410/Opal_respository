using System;

namespace Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads
{
    public class LearnerActivityLearningTrackingPayload
    {
        public Guid ItemId { get; set; }

        public string ItemType { get; set; }

        public string TrackingAction { get; set; }
    }
}
