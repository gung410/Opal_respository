using System;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Learner.Domain.Entities
{
    public class LearnerLearningPath : FullAuditedEntity, ISoftDelete
    {
        public string Title { get; set; }

        public Guid CreatedBy { get; set; }

        public string ThumbnailUrl { get; set; }

        public bool IsPublic { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsCreationOwner(Guid userId)
        {
            return CreatedBy == userId;
        }

        /// <summary>
        /// Publish learning path so that every learner can view the new path,
        /// by setting IsPublic flag = true.
        /// </summary>
        public void Publish()
        {
            IsPublic = true;
        }
    }
}
