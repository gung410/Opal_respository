using System;
using Microservice.Learner.Domain.ValueObject;

namespace Microservice.Learner.Application.Consumers
{
#pragma warning disable SA1402 // File may only contain a single type
    public class LearningTrackingMessage
    {
        public string EventName { get; set; }

        public Guid UserId { get; set; }

        public LearningTrackingModel Payload { get; set; }
    }

    public class LearningTrackingModel
    {
        public Guid ItemId { get; set; }

        public LearningTrackingType TrackingType { get; set; }

        public LearningTrackingAction TrackingAction { get; set; }
    }
#pragma warning restore SA1402 // File may only contain a single type
}
