using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Learner.Domain.Entities
{
    public class UserLike : AuditedEntity
    {
        public Guid ItemId { get; set; }

        public LearningTrackingType ItemType { get; set; }

        public Guid CreatedBy { get; set; }
    }
}
