using System;
using Microservice.NewsFeed.Domain.ValueObject;
using MongoDB.Bson.Serialization.Attributes;

namespace Microservice.NewsFeed.Domain.Entities
{
    /// <summary>
    /// Sync from Course table on the CAM module.
    /// </summary>
    public class Course
    {
        public const string CourseDetailUrl = "learner/detail/course";

        [BsonId]
        public Guid Id { get; set; }

        /// <summary>
        /// The identifier course.
        /// </summary>
        public Guid CourseId { get; set; }

        /// <summary>
        /// Name of the course.
        /// </summary>
        public string CourseName { get; set; }

        /// <summary>
        /// Thumbnail of the course.
        /// </summary>
        public string ThumbnailUrl { get; set; }

        public string LearningMode { get; set; }

        /// <summary>
        /// Code of course.
        /// </summary>
        public string CourseCode { get; set; }

        /// <summary>
        /// Description of course.
        /// </summary>
        public string Description { get; set; }

        public Guid? MOEOfficerId { get; set; }

        public string PDActivityType { get; set; }

        public DateTime? SubmittedDate { get; set; }

        /// <summary>
        /// <see cref="CourseType"/> of course.
        /// </summary>
        public CourseType CourseType { get; set; }

        /// <summary>
        /// The maximum number of times a user can take the course.
        /// </summary>
        public int MaxReLearningTimes { get; set; }

        public Guid? FirstAdministratorId { get; set; }

        public Guid? SecondAdministratorId { get; set; }

        public Guid? PrimaryApprovingOfficerId { get; set; }

        public Guid? AlternativeApprovingOfficerId { get; set; }

        /// <summary>
        /// The <see cref="CourseStatus"/> of course.
        /// </summary>
        public CourseStatus Status { get; set; }

        /// <summary>
        /// The <see cref="ContentStatus"/> of course.
        /// </summary>
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

        public DateTime CreatedDate { get; set; }

        public DateTime? ChangedDate { get; set; }

        /// <summary>
        /// Check the course name has changed.
        /// </summary>
        /// <param name="courseName">Name of the course.</param>
        /// <returns>Returns true if the old course name not equal to the new one,
        /// otherwise false.</returns>
        public bool IsDifferentCourseName(string courseName)
        {
            return CourseName != courseName;
        }

        /// <summary>
        /// Check the course name has changed.
        /// </summary>
        /// <param name="thumbnailUrl">ThumbnailUrl of the course.</param>
        /// <returns>Returns true if the old thumbnailUrl not equal to the new one,
        /// otherwise false.</returns>
        public bool IsDifferentThumbnailUrl(string thumbnailUrl)
        {
            return ThumbnailUrl != thumbnailUrl;
        }
    }
}
