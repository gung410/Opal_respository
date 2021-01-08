using System;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Learner.Domain.Entities
{
    public class LearnerLearningPathCourse : FullAuditedEntity, ISoftDelete
    {
        public Guid LearningPathId { get; set; }

        public Guid CourseId { get; set; }

        public int? Order { get; set; }

        public bool IsDeleted { get; set; }
    }
}
