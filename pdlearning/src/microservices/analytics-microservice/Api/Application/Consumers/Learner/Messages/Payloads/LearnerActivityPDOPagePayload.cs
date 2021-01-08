using System;
using Microservice.Analytics.Domain.ValueObject;

namespace Microservice.Analytics.Application.Consumers.Learner.Messages.Payloads
{
    public class LearnerActivityPDOPagePayload
    {
        public Guid ItemId { get; set; }

        public AnalyticLearnerActivityPDOPageTrackingType TrackingType { get; set; }
    }
}
