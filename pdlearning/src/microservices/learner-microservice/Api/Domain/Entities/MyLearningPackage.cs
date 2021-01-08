using System;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;
using Thunder.Platform.Core.Domain.Entities;

namespace Microservice.Learner.Domain.Entities
{
    public class MyLearningPackage : AuditedEntity, ISoftDelete
    {
        public Guid UserId { get; set; }

        /// <summary>
        /// Id of LectureInMyCourse. Only use when the Learning Package is inside a Course.
        /// Combine with UserId to specify MyLearningPackage.
        /// </summary>
        public Guid? MyLectureId { get; set; }

        /// <summary>
        /// Id of MyDigitalContent. Only use when the Learning Package is inside a standalone DigitalContent.
        /// Combine with UserId to specify MyLearningPackage.
        /// </summary>
        public Guid? MyDigitalContentId { get; set; }

        public LearningPackageType Type { get; set; }

        public string State { get; set; }

        public string LessonStatus { get; set; }

        public string CompletionStatus { get; set; }

        public string SuccessStatus { get; set; }

        public bool IsDeleted { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid? ChangedBy { get; set; }

        public int? TimeSpan { get; set; }
    }
}
