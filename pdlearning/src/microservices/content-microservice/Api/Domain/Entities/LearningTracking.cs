using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microservice.Content.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Content.Domain.Entities
{
    public class LearningTracking : AuditedEntity
    {
        public Guid ItemId { get; set; }

        public LearningTrackingType TrackingType { get; set; }

        public LearningTrackingAction TrackingAction { get; set; }

        public int TotalCount { get; set; }
    }
}
