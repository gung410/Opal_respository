using System;
using Microservice.Course.Domain.Interfaces;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Course.Domain.Entities
{
    public class LearningPathCourse : FullAuditedEntity, ISoftDelete, IOrderable
    {
        public Guid LearningPathId { get; set; }

        public Guid CourseId { get; set; }

        public int? Order { get; set; }

        public bool IsDeleted { get; set; }
    }
}
