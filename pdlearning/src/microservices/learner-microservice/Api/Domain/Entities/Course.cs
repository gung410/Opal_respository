using System;
using Conexus.Opal.AccessControl.Infrastructure;
using Microservice.Learner.Domain.ValueObject;
using Thunder.Platform.Core.Domain.Auditing;

namespace Microservice.Learner.Domain.Entities
{
    /// <summary>
    /// Sync from Course table on the CAM module.
    /// </summary>
    public class Course : AuditedEntity, IHasDepartment
    {
        /// <summary>
        /// Hard code MicroLearningTagId is obtained from the SAM module.
        /// </summary>
        public const string MicroLearningTagId = "db13d0f8-d595-11e9-baec-0242ac120004";

        public string CourseName { get; set; }

        public string LearningMode { get; set; }

        public string CourseCode { get; set; }

        public string Description { get; set; }

        public Guid? MOEOfficerId { get; set; }

        public string PDActivityType { get; set; }

        public DateTime? SubmittedDate { get; set; }

        public CourseType CourseType { get; set; }

        public int MaxReLearningTimes { get; set; }

        public Guid? FirstAdministratorId { get; set; }

        public Guid? SecondAdministratorId { get; set; }

        public Guid? PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        public CourseStatus Status { get; set; }

        public ContentStatus ContentStatus { get; set; }

        public DateTime? PublishedContentDate { get; set; }

        public DateTime? SubmittedContentDate { get; set; }

        public string Version { get; set; }

        public Guid CreatedBy { get; set; }

        public Guid ChangedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public DateTime? ApprovalContentDate { get; set; }

        public DateTime? PublishDate { get; set; }

        public DateTime? ArchiveDate { get; set; }

        public string Source { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? ExpiredDate { get; set; }

        public int DepartmentId { get; set; }

        public string ThumbnailUrl { get; set; }

        /// <summary>
        /// To check the course is published.
        /// </summary>
        /// <returns>Returns true if the status is <see cref="CourseStatus.Published"/>,
        /// otherwise false.</returns>
        public bool IsPublished()
        {
            return Status == CourseStatus.Published;
        }

        /// <summary>
        /// To check the course is un-published.
        /// </summary>
        /// <returns>Returns true if the status is <see cref="CourseStatus.Unpublished"/>,
        /// otherwise false.</returns>
        public bool IsUnPublished()
        {
            return Status == CourseStatus.Unpublished;
        }

        /// <summary>
        /// To check course is MicroLearning.
        /// </summary>
        /// <returns>Returns true if the PDActivityType is equals to <see cref="MicroLearningTagId"/>,
        /// otherwise false.</returns>
        public bool IsMicroLearning()
        {
            return PDActivityType == MicroLearningTagId;
        }
    }
}
