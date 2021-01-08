using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Learner.Domain.Entities
{
    public class LearningTracking : AuditedEntity
    {
        public Guid ItemId { get; set; }

        public LearningTrackingType TrackingType { get; set; }

        public LearningTrackingAction TrackingAction { get; set; }

        public int TotalCount { get; set; }
    }
}
